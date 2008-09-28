#region License
//=============================================================================
// System  : TRA Game.Networking
// File    : SendNetworkData.cs
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
// ============================================================================ 
#endregion
using System;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.GamerServices;

namespace EGGEngine.Networking
{
     public class SendNetworkData
    {
        NetworkHelper networkHelper = new NetworkHelper();
         /*
        /// <summary>
        /// Send all server data
        /// </summary>
        public void SendServerData()
        {
            if (networkHelper.serverPacketWriter.Length > 0)
            {
                // Send the combined data to everyone in the session
                LocalNetworkGamer server = (LocalNetworkGamer)networkHelper.session.Host;

                server.SendData(networkHelper.serverPacketWriter, SendDataOptions.InOrder);
            }
        }

        /// <summary>
        /// Send the Client Data
        /// </summary>
        public void SendClientData()
        {
            if (networkHelper.ClientPacketWriter.Length > 0)
            {
                // The first player is always running in the server...
                networkHelper.session.LocalGamers[0].SendData(networkHelper.ClientPacketWriter,
                    SendDataOptions.InOrder, networkHelper.session.Host);
            }
        }*/
    }
}
