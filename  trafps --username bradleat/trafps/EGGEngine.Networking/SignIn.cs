﻿#region License
//=============================================================================
// System  : TRA Game.Networking
// File    : SignIn.cs
// Author  : EV
// Note    : Copyright 2008, Portal Games, All Rights Reserved
// Compiler: Microsoft C#
//
// This file is for networking.
//
// This code is published under the Microsoft Reciprocal License (Ms-RL). A 
// copy of the license should be distributed with the code. It can also be found
// at the project website: http://www.CodePlex.com/trafps. This notice, the
// author's name, and all copyright notices must remain intact in all
// applications, documentation, and source files.
//
//
// ============================================================================ 
#endregion

using System;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.GamerServices;

namespace EGGEngine.Networking
{
     public class SignIn
    {
         /// <summary>
         /// Signs In a gamer
         /// </summary>
         /// <param name="paneCount"></param>
         /// <param name="onlineOnly"></param>
        public void SignInGamer(int paneCount, bool onlineOnly)
        {
            if (!Guide.IsVisible)
            {
                Guide.ShowSignIn(paneCount, onlineOnly);
            }
        }
    }
}
