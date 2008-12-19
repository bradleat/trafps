using System;

namespace EGGEditor01
{
    static class Program
    {
        [STAThread]
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
                  //EGGEditor form = new EGGEditor();  
                  //form.Show();  
                  //Game1 game = new Game1(form.getDrawSurface());
                  Game1 game = new Game1();
                  EGGEditor form1 = new EGGEditor(game);
                  Game1 game1 = new Game1(form1.getDrawSurface());
                  form1.Show();  
                  game1.Run();              
       }  
    }
}

