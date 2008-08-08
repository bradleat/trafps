#region License
//=============================================================================
// System  : TRA Game.Networking
// File    : NetworkHelper.cs
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
// Revision Number (add one each time you edit code) 2
// Sign:
// Bradley Leatherwood, Revision 1
// 
// Todos: Design, Add Spectator code.
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
        public void SignInGamer(int paneCount, bool onlineOnly)
        {
            if (!Guide.IsVisible)
            {
                Guide.ShowSignIn(paneCount, onlineOnly);
            }
        }
    }
}
