#region License
//=============================================================================
// System  : EGGEngine.Networking
// File    : NetworkSession.cs
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
// Revision Number: 0.2.0.0
// Revision Number: Major.Minor.Build.Bug
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.GamerServices;
#endregion

namespace EGGEngine.Networking
{
    public class SessionManager
    {
        NetworkHelper networkHelper;
        public SessionManager(Game game)
        {
            networkHelper = (NetworkHelper)
            game.Services.GetService(typeof(NetworkHelper));
        }

        /// <summary>
        /// 
        /// </summary>
        public IAsyncResult CreateSession(NetworkSessionType sessionType, int maxLocalGamers,
            int maxGamers, int PrivateGamerSlots, NetworkSessionProperties sessionProperties)
        {

            IAsyncResult asyncResult = Microsoft.Xna.Framework.Net.NetworkSession.BeginCreate(sessionType, maxLocalGamers, maxGamers, PrivateGamerSlots, sessionProperties, null, null);
            
            //networkHelper.NetworkGameSession = Microsoft.Xna.Framework.Net.NetworkSession.Create(
                //sessionType, maxLocalGamers, maxGamers, PrivateGamerSlots, sessionProperties);

            return asyncResult;
        }

        public IAsyncResult JoinSession(NetworkSessionType sessionType, int maxLocalGamers, NetworkSessionProperties sessionProperties)
        {
            IAsyncResult asyncResult = Microsoft.Xna.Framework.Net.NetworkSession.BeginFind(sessionType, maxLocalGamers, sessionProperties, null, null);

            return asyncResult;
            /*
            // Search for sessions.
            using (AvailableNetworkSessionCollection availableSessions =
                        Microsoft.Xna.Framework.Net.NetworkSession.Find(sessionType, maxLocalGamers, sessionProperties))
            {
                if (availableSessions.Count == 0)
                {
                    return;
                }

                // Join the first session we found.
                networkHelper.NetworkGameSession = Microsoft.Xna.Framework.Net.NetworkSession.Join(availableSessions[0]);
            }*/

        }
    }
}
