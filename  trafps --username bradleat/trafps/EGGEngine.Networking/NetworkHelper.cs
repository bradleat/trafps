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
//
// ============================================================================ 
#endregion

using System;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.GamerServices;

namespace EGGEngine.Networking
{
     public class NetworkHelper
    {
        // The Game Session
        public NetworkSession session = null;
        // Maximum 16 players
        public int maximumGamers = 16;
        // No split-scren, only remote players
        public int maximumLocalPlayers = 1;
        // PacketWriter/Reader to send/recieve packets
        public readonly PacketWriter serverPacketWriter = new PacketWriter();
        public readonly PacketReader serverPacketReader = new PacketReader();
        public readonly PacketWriter clientPacketWriter = new PacketWriter();
        public readonly PacketReader clientPacketReader = new PacketReader();
        // All sessions found
         public AvailableNetworkSessionCollection availableSessions;
         // The session we'll join
         public AvailableNetworkSession availableSession = null;

        /// <summary>
        /// The active network session
        /// </summary>
        public NetworkSession NetworkGameSession
        {
            get { return session; }
            set { session = value; }
        }

        /// <summary>
        /// Writer for the server data
        /// </summary>
        public PacketWriter ServerPacketWriter
        {
            get { return serverPacketWriter; }
        }

        /// <summary>
        /// Writer for the client data
        /// </summary>
        public PacketWriter ClientPacketWriter
        {
            get { return clientPacketWriter; }
        }

        /// <summary>
        /// Reader for the client data
        /// </summary>
        public PacketReader ClientPacketReader
        {
            get { return clientPacketReader; }
        }

        public PacketReader ServerPacketReader
        {
            get { return serverPacketReader; }
        }

        public void Update()
        {
            if (session != null)
            {
                // Updates the session
                session.Update();
            }
        }
        
    }
}
