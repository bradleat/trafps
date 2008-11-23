using System;
using TRA_Game;

namespace EGGEditor01
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
                  EGGEditor form = new EGGEditor();  
                  form.Show();  
                  Game1 game = new Game1(form.getDrawSurface());  
                  game.Run();              
       }  
    }
}

