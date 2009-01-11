using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;

namespace EGGEngine.Awards
{
    /// <summary>
    /// Contains all the awards for the game and manages everything about them. 
    /// Uses the Guide.NotificationPosition when drawing the announcements.
    /// </summary>
    public class AwardsComponent : DrawableGameComponent
    {
        // we keep a list of unlocked awards
        private readonly List<AwardProgress> currentAwards = new List<AwardProgress>();

        // a queue of recently unlocked awards we need to announce
        private readonly Queue<AwardProgress> notifyList = new Queue<AwardProgress>();

        // the length of time spent fading in and out announcements
        private const float AnnouncementFadeLength = .25f;

        // the length of time the announcement is at full opacity on the screen
        private const float AnnouncementLength = 2f;

        // sets the offset (in pixels from the top-left corner) of where to place the
        // award icon relative to the background texture
        private const int IconOffsetX = 11;
        private const int IconOffsetY = 8;

        // sets the location of the center of the gamertag in the announcement
        private const int GamertagCenterX = 161;
        private const int GamertagCenterY = 16;

        // sets the location of the center of the award name in the announcement
        private const int AwardCenterX = 161;
        private const int AwardCenterY = 40;

        // sets the location of the center of the award notification message
        private const int MessageCenterX = 161;
        private const int MessageCenterY = 64;
        private const string AwardUnlockedMessage = "Award Unlocked!";

        // a timer for announcements
        private float announcementTimer;

        // the background to use for the announcement
        private Texture2D announcementBackground;

        // a sound effect to play when a new announcement is going up
        private SoundEffect announcementSound;

        // a SpriteBatch used to draw everything
        private SpriteBatch spriteBatch;

        // the font used for rendering award names over the background
        private SpriteFont announcementFont;

        // whether or not to draw the announcement
        private bool showAnnouncement;

        /// <summary>
        /// Gets the list of all awards in the game.
        /// </summary>
        public List<Award> Awards { get; private set; }

        /// <summary>
        /// Raised when an award is unlocked.
        /// </summary>
        /// <remarks>
        /// The event is useful for games that unlock special content when awards
        /// are unlocked. This allows games to unlock awards from anywhere and have
        /// a single location listen for the events to respond accordingly.
        /// </remarks>
        public event EventHandler<AwardUnlockedEventArgs> AwardUnlocked;

        /// <summary>
        /// Creates a new AwardsComponent for the given game.
        /// </summary>
        /// <param name="game">The Game in which the component will run.</param>
        public AwardsComponent(Game game)
            : base(game)
        {
            Awards = new List<Award>();

            // we want awards to draw absolutely last so we set the draw order to max
            DrawOrder = int.MaxValue;
        }

        protected override void LoadContent()
        {
            // load each award's texture
            Awards.ForEach(a => a.LoadTexture(Game.Content));

            // load the announcement texture, sound, and font
            announcementBackground = Game.Content.Load<Texture2D>("award-background");
            announcementSound = Game.Content.Load<SoundEffect>("award-sound");
            announcementFont = Game.Content.Load<SpriteFont>("award-font");

            // create the SpriteBatch
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        public override void Update(GameTime gameTime)
        {
            // if there are unlocked awards left to announce
            if (notifyList.Count > 0)
            {
                // if we're not currently showing an announcement
                if (!showAnnouncement)
                {
                    // reset the timer and show the announcement
                    announcementTimer = 0f;
                    showAnnouncement = true;

                    // play our sound effect
                    announcementSound.Play();
                }

                // otherwise if we are showing an announcement
                else
                {
                    // add to the timer
                    announcementTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                    // if the timer is past the announcement period (fade in, on screen, fade out)
                    if (announcementTimer >= (AnnouncementFadeLength * 2) + AnnouncementLength)
                    {
                        // hide the announcement
                        showAnnouncement = false;

                        // dequeue the award since it has been announced
                        notifyList.Dequeue();
                    }
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            // if an announcement is being shown
            if (showAnnouncement)
            {
                // default to full opacity
                float opacity = 1f;

                // figure out if we're fading in or out and set the opacity accordingly
                if (announcementTimer < AnnouncementFadeLength)
                    opacity = announcementTimer / AnnouncementFadeLength;
                else if (announcementTimer > AnnouncementFadeLength + AnnouncementLength)
                    opacity = 1f - ((announcementTimer - AnnouncementLength - AnnouncementFadeLength) / AnnouncementFadeLength);

                // figure out the bounds based on the texture size and the Guide.NotificationPosition
                Rectangle announcementBounds = CalculateAnnouncementBounds();

                // make the colors
                Color color = new Color(Color.White, opacity);
                Color fontColor = new Color(Color.Black, opacity);

                // start drawing. we use SaveState in case you have 3D rendering so it won't go bad
                AwardProgress awardProgress = notifyList.Peek();
                Award award = awardProgress.Award;
                spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Deferred, SaveStateMode.SaveState);

                // draw the background
                spriteBatch.Draw(announcementBackground, announcementBounds, color);

                // draw the icon
                spriteBatch.Draw(award.Texture, GetIconBounds(announcementBounds), color);

                // draw the award name
                spriteBatch.DrawString(announcementFont, awardProgress.Gamertag, GetGamertagPosition(announcementBounds, awardProgress.Gamertag), fontColor);
                spriteBatch.DrawString(announcementFont, award.Name, GetNamePosition(announcementBounds), fontColor);
                if (awardProgress.IsUnlocked)
                {
                    spriteBatch.DrawString(announcementFont, AwardUnlockedMessage, GetMessagePosition(announcementBounds, AwardUnlockedMessage), fontColor);
                }
                else
                {
                    string message = string.Format("{0}/{1}", awardProgress.Progress, award.ProgressNeeded);
                    spriteBatch.DrawString(announcementFont, message, GetMessagePosition(announcementBounds, message), fontColor);
                }

                // done drawing
                spriteBatch.End();
            }

            base.Draw(gameTime);
        }

        /// <summary>
        /// Gets the location where to draw the gamertag.
        /// </summary>
        /// <param name="announcementBounds">The announcement bounds.</param>
        /// <param name="gamertag">The gamertag.</param>
        /// <returns>The new Vector2 position for the gamertag.</returns>
        private Vector2 GetGamertagPosition(Rectangle announcementBounds, string gamertag)
        {
            return GetTextPosition(announcementBounds, gamertag, GamertagCenterX, GamertagCenterY);
        }

        /// <summary>
        /// Gets the location where to draw the award name.
        /// </summary>
        /// <param name="announcementBounds">The announcement bounds.</param>
        /// <returns>The new Vector2 position for the award name.</returns>
        private Vector2 GetNamePosition(Rectangle announcementBounds)
        {
            return GetTextPosition(announcementBounds, notifyList.Peek().Award.Name, AwardCenterX, AwardCenterY);
        }

        /// <summary>
        /// Gets the location where to draw the award message.
        /// </summary>
        /// <param name="announcementBounds">The announcement bounds.</param>
        /// <param name="message">The award message.</param>
        /// <returns>The new Vector2 position for the award message.</returns>
        private Vector2 GetMessagePosition(Rectangle announcementBounds, string message)
        {
            return GetTextPosition(announcementBounds, message, MessageCenterX, MessageCenterY);
        }

        /// <summary>
        /// Gets the location where to draw text in an award notification.
        /// </summary>
        /// <param name="announcementBounds">The announcement bounds.</param>
        /// <param name="text">The text to display.</param>
        /// <param name="centerX">Horizontal center of where to render.</param>
        /// <param name="centerY">Vertical center of where to render.</param>
        /// <returns>The new Vector2 position for the text.</returns>
        private Vector2 GetTextPosition(Rectangle announcementBounds, string text, int centerX, int centerY)
        {
            // get the center position of the text using our 
            // centerX and centerY offsets
            Vector2 position = new Vector2(
               announcementBounds.X + centerX,
               announcementBounds.Y + centerY);

            // measure the text we'll be drawing
            Vector2 stringSize = announcementFont.MeasureString(text);

            // subtract half of the size from the position to center it
            position -= stringSize / 2f;

            // return the position, but cast the X and Y to integers to avoid filtering
            return new Vector2((int)position.X, (int)position.Y);
        }

        /// <summary>
        /// Gets the bounds for the award icon.
        /// </summary>
        /// <param name="announcementBounds">The announcement bounds.</param>
        /// <returns>The new Rectangle bounds for the award icon.</returns>
        private Rectangle GetIconBounds(Rectangle announcementBounds)
        {
            // simply offset the announcement bounds position by the icon offsets
            // and set the width and height to 64
            return new Rectangle(
               announcementBounds.X + IconOffsetX,
               announcementBounds.Y + IconOffsetY,
               64,
               64);
        }

        /// <summary>
        /// Calculates the bounds for the announcement based on the Guide.NotificationPosition property
        /// </summary>
        /// <returns>The new Rectangle bounds for the announcement.</returns>
        private Rectangle CalculateAnnouncementBounds()
        {
            // the amount of the screen to buffer on the sides for the safe zone
            const float safeZoneBuffer = .1f;

            // the actual buffer sizes for the width and height
            int widthBuffer = (int)(GraphicsDevice.PresentationParameters.BackBufferWidth * safeZoneBuffer);
            int heightBuffer = (int)(GraphicsDevice.PresentationParameters.BackBufferHeight * safeZoneBuffer);

            // start with a basic rectangle the size of our texture
            Rectangle announcementBounds = new Rectangle(0, 0, announcementBackground.Width, announcementBackground.Height);

            // figure out all six of our values now to make our switch statement cleaner
            int topY = heightBuffer;
            int centerY = GraphicsDevice.PresentationParameters.BackBufferHeight / 2 - announcementBounds.Height / 2;
            int bottomY = GraphicsDevice.PresentationParameters.BackBufferHeight - heightBuffer - announcementBounds.Height;

            int leftX = widthBuffer;
            int centerX = GraphicsDevice.PresentationParameters.BackBufferWidth / 2 - announcementBounds.Width / 2;
            int rightX = GraphicsDevice.PresentationParameters.BackBufferWidth - widthBuffer - announcementBounds.Width;

            // set the bounds X and Y based on the notification position
            switch (Guide.NotificationPosition)
            {
                case NotificationPosition.BottomCenter:
                    announcementBounds.X = centerX;
                    announcementBounds.Y = bottomY;
                    break;
                case NotificationPosition.BottomLeft:
                    announcementBounds.X = leftX;
                    announcementBounds.Y = bottomY;
                    break;
                case NotificationPosition.BottomRight:
                    announcementBounds.X = rightX;
                    announcementBounds.Y = bottomY;
                    break;
                case NotificationPosition.Center:
                    announcementBounds.X = centerX;
                    announcementBounds.Y = centerY;
                    break;
                case NotificationPosition.CenterLeft:
                    announcementBounds.X = leftX;
                    announcementBounds.Y = centerY;
                    break;
                case NotificationPosition.CenterRight:
                    announcementBounds.X = rightX;
                    announcementBounds.Y = centerY;
                    break;
                case NotificationPosition.TopCenter:
                    announcementBounds.X = centerX;
                    announcementBounds.Y = topY;
                    break;
                case NotificationPosition.TopLeft:
                    announcementBounds.X = leftX;
                    announcementBounds.Y = topY;
                    break;
                case NotificationPosition.TopRight:
                    announcementBounds.X = rightX;
                    announcementBounds.Y = topY;
                    break;
            }

            return announcementBounds;
        }

        /// <summary>
        /// Helper function. Acquires the AwardProgress for the given award and gamer.
        /// </summary>
        /// <param name="award">The award to search for.</param>
        /// <param name="gamertag">The gamer unlocking the award.</param>
        /// <param name="makeIfNotFound">
        /// True if a new AwardProgress should be created if it does not exist.
        /// </param>
        /// <returns>The appropriate AwardProgress object, or null if not found or created.</returns>
        private AwardProgress GetAwardProgress(Award award, string gamertag, bool makeIfNotFound)
        {
            // Try to find the AwardProgress.
            AwardProgress findProgress = currentAwards.Find(a => a.Award == award &&
                                                                 a.Gamertag == gamertag);

            // If we didn't find it, make a new one if requested.
            if (findProgress == null && makeIfNotFound)
            {
                findProgress = new AwardProgress
                {
                    Award = award,
                    Gamertag = gamertag
                };
                currentAwards.Add(findProgress);
            }

            return findProgress;
        }

        /// <summary>
        /// Retrieves an award by name.
        /// </summary>
        /// <param name="name">The name of the award to retrieve.</param>
        /// <returns>The Award instance if a match was found, null otherwise.</returns>
        public Award GetAwardByName(string name)
        {
            // find the award based on name.
            return Awards.Find(a => a.Name == name);
        }

        /// <summary>
        /// Determines if a given award has been unlocked.
        /// </summary>
        /// <param name="award"></param>
        /// <param name="gamertag"></param>
        /// <returns></returns>
        public bool IsAwardUnlocked(Award award, string gamertag)
        {
            // Find the AwardProgress, don't create it if it isn't there.
            AwardProgress findAward = GetAwardProgress(award, gamertag, false);

            // Return whether we found it and if it's unlocked.
            return findAward != null && findAward.IsUnlocked;
        }

        /// <summary>
        /// Unlocks an award.
        /// </summary>
        /// <param name="award">The award to unlock.</param>
        /// <param name="gamertag">The gamer who unlocked the award.</param>
        public void UnlockAward(Award award, string gamertag)
        {
            // Make sure the award is in our list.
            if (Awards.Contains(award))
            {
                // Get the progress, make a new one if not found.
                AwardProgress findAward = GetAwardProgress(award, gamertag, true);

                // Only unlock if it hasn't been yet.
                if (!findAward.IsUnlocked)
                {
                    // Set the award's progress.
                    findAward.Progress = award.ProgressNeeded;

                    // Add it to the notification queue.
                    notifyList.Enqueue(findAward);

                    // Invoke the event.
                    if (AwardUnlocked != null)
                        AwardUnlocked(this, new AwardUnlockedEventArgs(award, gamertag));
                }
            }
        }

        /// <summary>
        /// Adds progress points to an award for a gamer, unlocking it if needed.
        /// </summary>
        /// <param name="award">The award to add progress.</param>
        /// <param name="gamertag">The gamer progressing the award.</param>
        /// <param name="progress">The number of progress points obtained.</param>
        public void AddAwardProgress(Award award, string gamertag, int progress)
        {
            // Make sure the award is in our list.
            if (Awards.Contains(award))
            {
                // Get the progress, make a new one if not found.
                AwardProgress findAward = GetAwardProgress(award, gamertag, true);

                // Only add points if it hasn't been unlocked yet.
                if (!findAward.IsUnlocked)
                {
                    // Get the last progress increment reached.
                    int lastIncrement = findAward.Progress / award.ProgressIncrement;

                    // Add to the award's progress and get the new progress increment.
                    findAward.Progress += progress;
                    int nextIncrement = findAward.Progress / award.ProgressIncrement;

                    // First, check again if it is unlocked.
                    if (findAward.IsUnlocked)
                    {
                        // Add it to the notification queue.
                        notifyList.Enqueue(findAward);

                        // Invoke the event.
                        if (AwardUnlocked != null)
                            AwardUnlocked(this, new AwardUnlockedEventArgs(award, gamertag));
                    }
                    // Otherwise, check for a partial progress notification.
                    else if (lastIncrement < nextIncrement)
                    {
                        // Round down the progress points and add it to the notification queue.
                        int roundedProgress = nextIncrement * award.ProgressIncrement;
                        notifyList.Enqueue(new AwardProgress
                        {
                            Award = award,
                            Gamertag = gamertag,
                            Progress = roundedProgress
                        });
                    }
                }
            }
        }

        /// <summary>
        /// Saves a list of the unlocked award names to a file. This is not the best
        /// route on Windows where a user could easily edit the file, but on Xbox
        /// it is sufficient given that the hard drive is not easily accessible.
        /// </summary>
        /// <param name="file">The file path to which the data is saved.</param>
        public void SaveAwardProgress(string file)
        {
            // delete the file if it exists
            if (File.Exists(file))
                File.Delete(file);

            // use a StreamWriter to write the name of each unlocked award to the file
            using (StreamWriter writer = new StreamWriter(file))
            {
                currentAwards.ForEach(delegate(AwardProgress a)
                {
                    writer.WriteLine(a.Award.Name);
                    writer.WriteLine(a.Gamertag);
                    writer.WriteLine(a.Progress);
                });
            }
        }

        /// <summary>
        /// Loads a list of unlocked award names from a file.
        /// </summary>
        /// <param name="file">The file path from which the data is read.</param>
        public void LoadAwardProgress(string file)
        {
            // clear the list of unlocked awards
            currentAwards.Clear();

            // use a StreamReader to read in the data
            using (StreamReader reader = new StreamReader(file))
            {
                // loop while we're not at the end of the stream
                while (!reader.EndOfStream)
                {
                    // get an award by name where each line is an award
                    Award award = GetAwardByName(reader.ReadLine());
                    string gamertag = reader.ReadLine();
                    int progress = Convert.ToInt32(reader.ReadLine());

                    // if the award is not null, add it to our unlocked awards list
                    if (award != null)
                        currentAwards.Add(new AwardProgress
                        {
                            Award = award,
                            Gamertag = gamertag,
                            Progress = progress
                        });
                }
            }
        }
    }
}