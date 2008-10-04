#region License
//=============================================================================
// System  : TRA Game.Networking
// File    : Session.cs
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

#region Revision Number
// Revision Number: 0.1.0.0
// Revision Number: Major.Minor.Build.Bug
#endregion

#region Using Statements
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
#endregion

namespace EGGEngine.Networking
{
     public class Session
    {
         NetworkHelper networkHelper = new NetworkHelper();
         HandleEvents eventHandler = new HandleEvents();
/*
        public void CreateSession(NetworkSessionType sessionType, int maxLocalPlayers, int maxGamers)
        {
            if (networkHelper.session == null)
            {
                networkHelper.session = NetworkSession.Create(sessionType, maxLocalPlayers, maxGamers);

                // If the host goes out, another machine will asume as a new host
                networkHelper.session.AllowHostMigration = true;
                // Allow players to join a game in progress
                networkHelper.session.AllowJoinInProgress = true;
            }
        }

        /// <summary>
        /// Joins an existing network session.
        /// </summary>
        public void JoinSession(NetworkSessionType sessionType, int maxLoxalGamers,
            NetworkSessionProperties sessionProperties) 
        {

            try
            {
                // Search for sessions.
                using (AvailableNetworkSessionCollection availableSessions =
                            NetworkSession.Find(sessionType, maxLoxalGamers, sessionProperties)) 
                {
                    if (availableSessions.Count == 0)
                    {
                        errorMessage = "No network sessions found.";
                        return;
                    }

                    // Join the first session we found.
                    networkHelper.NetworkGameSession = NetworkSession.Join(availableSessions[0]);
                    
                }
            }
            catch (Exception e)
            {
                errorMessage = e.Message;
            }
        }
        */

    }
}
