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
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using EGGEngine.Networking;

namespace EGGEngine.Networking
{
     public class Session
    {
         NetworkHelper networkHelper = new NetworkHelper();
         HandleEvents eventHandler = new HandleEvents();

        public void CreateSession(NetworkSessionType sessionType, int maxLocalPlayers, int maxGamers, 
            int privateSlots, NetworkSessionProperties properties)
        {
            if (networkHelper.session == null)
            {
                networkHelper.session = NetworkSession.Create(sessionType, maxLocalPlayers, 
                    maxGamers, privateSlots, properties);

                // If the host goes out, another machine will asume as a new host
                networkHelper.session.AllowHostMigration = true;
                // Allow players to join a game in progress
                networkHelper.session.AllowJoinInProgress = true;

                eventHandler.HookSessionEvents();
            }
        }

        public void FindSession(NetworkSessionType sessionType, int maxLocalPlayers, NetworkSessionProperties properties)
        {
            // all sessions found
            AvailableNetworkSessionCollection availableSessions;
            // The session we'll join
            AvailableNetworkSession availableSession = null;

            availableSessions = NetworkSession.Find(sessionType, maxLocalPlayers, properties);

            // Get a session with available gamer slots
            foreach (AvailableNetworkSession  curSession in availableSessions)
            {
                int TotalSessionSlots = curSession.OpenPublicGamerSlots + curSession.OpenPrivateGamerSlots;
                if (TotalSessionSlots > curSession.CurrentGamerCount)
                {
                    availableSession = curSession;
                }
            }

            // if a session was found, connect to it
            if (availableSession != null)
            {
                networkHelper.session = NetworkSession.Join(availableSession);
            }
        }
        

    }
}
