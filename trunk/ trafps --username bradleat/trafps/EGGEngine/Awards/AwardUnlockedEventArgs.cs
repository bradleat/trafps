using System;

namespace EGGEngine.Awards
{
    /// <summary>
    /// A simple event argument class fired when an achievement is unlocked.
    /// </summary>
    public class AwardUnlockedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the Award that was unlocked.
        /// </summary>
        public Award Award { get; private set; }

        /// <summary>
        /// Gets the gamer who unlocked the award.
        /// </summary>
        public string Gamertag { get; private set; }

        /// <summary>
        /// Creates a new AwardUnlockedEventArgs with the given award.
        /// </summary>
        /// <param name="award">The award that was unlocked.</param>
        /// <param name="gamertag">The gamer who unlocked it.</param>
        public AwardUnlockedEventArgs(Award award, string gamertag)
        {
            Award = award;
            Gamertag = gamertag;
        }
    }
}