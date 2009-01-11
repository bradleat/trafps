using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace EGGEngine.Awards
{
    /// <summary>
    /// Represents a single Award the user can unlock.
    /// </summary>
    public class Award
    {
        /// <summary>
        /// Gets or sets the name of the Award.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the asset name for the texture. Each texture will be fit into
        /// a 64x64 area so being square is ideal.
        /// </summary>
        public string TextureAssetName { get; set; }

        /// <summary>
        /// Gets the texture for the Award.
        /// </summary>
        public Texture2D Texture { get; private set; }

        /// <summary>
        /// Gets or sets the progress points needed to unlock the award.
        /// </summary>
        public int ProgressNeeded { get; set; }

        /// <summary>
        /// Gets or sets the progress point increment between each award notification.
        /// </summary>
        public int ProgressIncrement { get; set; }

        /// <summary>
        /// Default constructor. Give default values to prevent division by zero.
        /// </summary>
        public Award()
        {
            ProgressNeeded = 1;
            ProgressIncrement = 1;
        }

        /// <summary>
        /// Loads the texture to represent the award.
        /// </summary>
        /// <param name="content">The ContentManager to use for loading.</param>
        public void LoadTexture(ContentManager content)
        {
            Texture = content.Load<Texture2D>(TextureAssetName);
        }
    }
}