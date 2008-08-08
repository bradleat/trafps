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

namespace TRA_Game.Networking
{
     public class NetworkHelper
    {
        // The Game Session
        private NetworkSession session;
        // Maximum 16 players
        private int maximumGamers = 16;
        // No split-scren, only remote players
        private int maximumLocalPlayers = 1;
        // Tracks the status of the asynchronous searching
        IAsyncResult AsyncSessionFind = null;
        // PacketWriter/Reader to send/recieve packets
        private readonly PacketWriter serverPacketWriter = new PacketWriter();
        private readonly PacketReader serverPacketReader = new PacketReader();
        private readonly PacketWriter clientPacketWriter = new PacketWriter();
        private readonly PacketReader clientPacketReader = new PacketReader();

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

        /// <summary>
        /// Send all server data
        /// </summary>
        public void SendServerData()
        {
            if (serverPacketWriter.Length > 0)
            {
                // Send the combined data to everyone in the session
                LocalNetworkGamer server = (LocalNetworkGamer) session.Host;

                server.SendData(serverPacketWriter, SendDataOptions.InOrder);
            }
        }

        public NetworkGamer ReadServerData(LocalNetworkGamer gamer)
        {
            NetworkGamer sender;

            // Read a single packet from the network
            gamer.ReceiveData(serverPacketReader, out sender);
            return sender;
        }

        /// <summary>
        /// Send the Client Data
        /// </summary>
        public void SendClientData()
        {
            if (ClientPacketWriter.Length > 0)
            {
                // The first player is always running in the server...
                session.LocalGamers[0].SendData(ClientPacketWriter,
                    SendDataOptions.InOrder, session.Host);
            }
        }

        public NetworkGamer ReadClientData(LocalNetworkGamer gamer)
        {
            NetworkGamer sender;

            // Read a single packet from the network
            gamer.ReceiveData(ClientPacketReader, out sender);
            return sender;
        }

        public void SetPlayerReady()
        {
            foreach (LocalNetworkGamer gamer in session.LocalGamers)
            {
                gamer.IsReady = true;
            }
        }


        public void session_SessionFound(IAsyncResult result)
        {
            // All sessions found
            AvailableNetworkSessionCollection availableSessions;
            // The session we will join
            AvailableNetworkSession availableSession = null;

            if (AsyncSessionFind.IsCompleted)
            {
                availableSessions = NetworkSession.EndFind(result);

                // Look for a session with available gamer slots
                foreach (AvailableNetworkSession  curSession in availableSessions)
                {
                    int TotalSessionSlots = curSession.OpenPublicGamerSlots +
                        curSession.OpenPrivateGamerSlots;
                    if (TotalSessionSlots > curSession.CurrentGamerCount)
                    {
                        availableSession = curSession;
                    }
                }

                // If a session was found, connect to it
                if (availableSession != null)
                {
                    session = NetworkSession.Join(availableSession);
                }
                else
                {
                    // Todo: add spectator code

                    // Reset the session finding result
                    AsyncSessionFind = null;
                }
            }
        }

        public void AsyncFindSystemLinkSession()
        {
            if (AsyncSessionFind == null)
            {
                // Asynchoronously finds a session
                AsyncSessionFind = NetworkSession.BeginFind(NetworkSessionType.SystemLink, maximumLocalPlayers, null,
                    new AsyncCallback(session_SessionFound), null);
            }
        }

        public void Update()
        {
            if (session != null)
            {
                // Updates the session
                session.Update();
            }
        }
        public void CreateSystemLinkSession()
        {
            if (session == null)
            {
                // Creates a System Link Session
                session = NetworkSession.Create(NetworkSessionType.SystemLink, 
                    maximumLocalPlayers, maximumGamers);

                // If the host goes out, another machine will assume as new host
                session.AllowHostMigration = true;
                // Allow players to join a game in progress
                session.AllowJoinInProgress = true;

                // Event hooks
                session.GamerJoined += new EventHandler<GamerJoinedEventArgs>(session_GamerJoined);
                session.GamerLeft += new EventHandler<GamerLeftEventArgs>(session_GamerLeft);
                session.GameStarted += new EventHandler<GameStartedEventArgs>(session_GameStarted);
                session.GameEnded += new EventHandler<GameEndedEventArgs>(session_GameEnded);
                session.SessionEnded += new EventHandler<NetworkSessionEndedEventArgs>(session_SessionEnded);
                session.HostChanged += new EventHandler<HostChangedEventArgs>(session_HostChanged);
            }
        }
        void session_GamerJoined(object sender, GamerJoinedEventArgs e)
        {
            // Todo: add events
        }

        void session_GamerLeft(object sender, GamerLeftEventArgs e)
        {
            // Todo: add events
        }

        void session_GameStarted(object sender, GameStartedEventArgs e)
        {
            // Todo: add events
        }

        void session_GameEnded(object sender, GameEndedEventArgs e)
        {
            // Todo: add events
        }

        void session_SessionEnded(object sender, NetworkSessionEndedEventArgs e)
        {
            // Todo: add events
        }

        void session_HostChanged(object sender, HostChangedEventArgs e)
        {
            // Todo: add events
        }
       
        public void SignInGamer()
        {
            if (!Guide.IsVisible)
            {
                Guide.ShowSignIn(1, false);
            }
        }
    }
}
