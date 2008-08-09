using System;

namespace NetworkingTestGame
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (NetworkingTestGame game = new NetworkingTestGame())
            {
                game.Run();
            }
        }
    }
}

