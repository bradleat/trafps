#region License
//=============================================================================
// System  : EGGEngine.Networking
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

#region Revision Number
// Revision Number: 0.2.0.0
// Revision Number: Major.Minor.Build.Bug
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.GamerServices;
#endregion


namespace EGGEngine.Networking
{
    public class NetworkHelper
    {

        private readonly Microsoft.Xna.Framework.Net.PacketWriter serverPacketWriter = new Microsoft.Xna.Framework.Net.PacketWriter();
        private readonly Microsoft.Xna.Framework.Net.PacketReader serverPacketReader = new Microsoft.Xna.Framework.Net.PacketReader();
        private readonly Microsoft.Xna.Framework.Net.PacketWriter clientPacketWriter = new Microsoft.Xna.Framework.Net.PacketWriter();
        private readonly Microsoft.Xna.Framework.Net.PacketReader clientPacketReader = new Microsoft.Xna.Framework.Net.PacketReader();

        private Microsoft.Xna.Framework.Net.NetworkSession networkSession;
        /// <summary>
        /// The active network session
        /// </summary>
        public Microsoft.Xna.Framework.Net.NetworkSession NetworkGameSession
        {
            get { return networkSession; }
            set { networkSession = value; }
        }

        /// <summary>
        /// Writer for the server data
        /// </summary>
        public Microsoft.Xna.Framework.Net.PacketWriter ServerPacketWriter
        {
            get { return serverPacketWriter; }
        }

        /// <summary>
        /// Writer for the client data
        /// </summary>
        public Microsoft.Xna.Framework.Net.PacketWriter ClientPacketWriter
        {
            get { return clientPacketWriter; }
        }

        /// <summary>
        /// Reader for the client data
        /// </summary>
        public Microsoft.Xna.Framework.Net.PacketReader ClientPacketReader
        {
            get { return clientPacketReader; }
        }

        /// <summary>
        /// Reader for the server data
        /// </summary>
        public Microsoft.Xna.Framework.Net.PacketReader ServerPacketReader
        {
            get { return serverPacketReader; }
        }



        /// <summary>
        /// Send all server data
        /// </summary>
        public void SendServerData()
        {
            if (ServerPacketWriter.Length > 0)
            {
                // Send the combined data to everyone in the session.
                LocalNetworkGamer server = (LocalNetworkGamer)networkSession.Host;

                server.SendData(ServerPacketWriter, SendDataOptions.InOrder);
            }
        }

        /// <summary>
        /// Read server data
        /// </summary>
        public NetworkGamer ReadServerData(LocalNetworkGamer gamer)
        {
            NetworkGamer sender;

            // Read a single packet from the network.
            gamer.ReceiveData(ServerPacketReader, out sender);
            return sender;
        }

        /// <summary>
        /// Send all client data
        /// </summary>
        public void SendClientData()
        {
            if (ClientPacketWriter.Length > 0)
            {
                // The first player is always running in the server...
                networkSession.LocalGamers[0].SendData(clientPacketWriter,
                                                       SendDataOptions.InOrder,
                                                       networkSession.Host);
            }
        }

        /// <summary>
        /// Read the Client Data
        /// </summary>
        public NetworkGamer ReadClientData(LocalNetworkGamer gamer)
        {
            NetworkGamer sender;

            // Read a single packet from the network.
            gamer.ReceiveData(ClientPacketReader, out sender);
            return sender;
        }
    }
}
