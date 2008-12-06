#region License
//=============================================================================
// System  : Game
// File    : Program.cs
// Author  : Dustin
// Note    : Copyright 2008, Portal Games, All Rights Reserved
// Compiler: Microsoft C#
//
// This file is for Try Catch and exception handeling. This is also the main
// entry point for the game. 
// 
//
// This code is published under the Microsoft Reciprocal License (Ms-RL). A 
// copy of the license should be distributed with the code. It can also be found
// at the project website: http://www.CodePlex.com/trafps. This notice, the
// author's name, and all copyright notices must remain intact in all
// applications, documentation, and source files.
//
// 
// Todos: Add in Try-Catch
//
// ============================================================================
#endregion

using System;

namespace TRA_Game
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (TRA_Game game = new TRA_Game())
            {
                game.Run();
            }
        }
    }
}

