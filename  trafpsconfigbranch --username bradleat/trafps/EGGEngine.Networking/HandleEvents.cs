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
    public class HandleEvents
    {
        NetworkHelper networkHelper;

        public void HookSessionEvents()
        {
            networkHelper.session.GamerJoined += new EventHandler<GamerJoinedEventArgs>(session_GamerJoined);
            networkHelper.session.GamerLeft += new EventHandler<GamerLeftEventArgs>(session_GamerLeft);
            networkHelper.session.GameStarted += new EventHandler<GameStartedEventArgs>(session_GameStarted);
            networkHelper.session.GameEnded += new EventHandler<GameEndedEventArgs>(session_GameEnded);
            networkHelper.session.SessionEnded += new EventHandler<NetworkSessionEndedEventArgs>(session_SessionEnded);
            networkHelper.session.HostChanged += new EventHandler<HostChangedEventArgs>(session_HostChanged);
            
        }

        void session_GamerJoined(object sender, GamerJoinedEventArgs e)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        void session_GamerLeft(object sender, GamerLeftEventArgs e)
        {
            
        }

        void session_GameStarted(object sender, GameStartedEventArgs e)
        {
            
        }

        void session_HostChanged(object sender, HostChangedEventArgs e)
        {
            
        }

        void session_SessionEnded(object sender, NetworkSessionEndedEventArgs e)
        {
            
        }

        void session_GameEnded(object sender, GameEndedEventArgs e)
        {
            
        }
        
        
        
    }
}
