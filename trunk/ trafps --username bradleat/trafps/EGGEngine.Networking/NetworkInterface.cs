#region License
//=============================================================================
// System  : EGGEngine.Networking
// File    : NetworkInterface.cs
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
using EGGEngine.Networking;
#endregion

namespace EGGEngine.Networking
{
    public class NetworkInterface
    {
        NetworkHelper networkHelper;



        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int InitNetwork(Game game)
        {
            networkHelper = (NetworkHelper)
            game.Services.GetService(typeof(NetworkHelper));

            return 0;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IAsyncResult CreateNetwork(Game game, NetworkSessionType sessionType, int maxLocalGamers, int maxGamers, int privateGamerSlots, NetworkSessionProperties sessionProperties, bool AllowHostMigration, bool AllowJoinInProgress)
        {
            SessionManager sessionManager = new SessionManager(game);

            IAsyncResult asyncResult = sessionManager.CreateSession(sessionType, maxLocalGamers, maxGamers, privateGamerSlots, sessionProperties);
            /*
            if (networkHelper.NetworkGameSession != null)
            {
                if (AllowHostMigration == true)
                    networkHelper.NetworkGameSession.AllowHostMigration = true;

                if (AllowJoinInProgress == true)
                    networkHelper.NetworkGameSession.AllowJoinInProgress = true;
                return asyncResult;
            }
            else
            {
                //throw new Exception("Session was not Created");
                return asyncResult;

            }*/
            return asyncResult;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int FrameNetwork(int temp, int etat)
        {

            if (!networkHelper.NetworkGameSession.IsHost)
            {
                networkHelper.SendClientData();
            }
            else
            {
                // If we are the server, transmit the game state
                networkHelper.SendServerData();
            }

            // Pump the data
            networkHelper.NetworkGameSession.Update();


            //the following part need one more class
            // Read any incoming network packets.
            foreach (LocalNetworkGamer gamer in
                    networkHelper.NetworkGameSession.LocalGamers)
            {
                // Keep reading as long as incoming packets are available.
                while (gamer.IsDataAvailable)
                {
                    NetworkGamer sender;
                    if (gamer.IsHost)
                    {
                        sender = networkHelper.ReadClientData(gamer);
                        if (!sender.IsLocal)
                        {
                            while (networkHelper.ClientPacketReader.PeekChar() != -1)
                            {
                                char header = networkHelper.ClientPacketReader.ReadChar();
                                if (header == 'P')
                                {
                                    etat = networkHelper.ClientPacketReader.ReadInt32();
                                }
                            }
                        }
                    }
                    else
                    {
                        sender = networkHelper.ReadServerData(gamer);
                        if (!sender.IsLocal)
                        {
                            while (networkHelper.ServerPacketReader.PeekChar() != -1)
                            {
                                char header = networkHelper.ServerPacketReader.ReadChar();

                                if (header == 'S') etat = -1;

                                else if (header == 'P')
                                {
                                    etat = networkHelper.ServerPacketReader.ReadInt32();
                                }
                            }
                        }
                    }

                }
            }

            //time to send something
            if (networkHelper.NetworkGameSession.IsHost)
            {
                networkHelper.ServerPacketWriter.Write('P');
                networkHelper.ServerPacketWriter.Write(temp);
            }
            else
            {
                networkHelper.ClientPacketWriter.Write('P');
                networkHelper.ClientPacketWriter.Write(temp);
            }

            return etat;
            //return 0;
        }

        int HandleClientData()
        {
            return networkHelper.ClientPacketReader.ReadInt32();
        }

        int HandleServerData()
        {
            while (networkHelper.ServerPacketReader.PeekChar() != -1)
            {
                char header = networkHelper.ServerPacketReader.ReadChar();
                switch (header)
                {
                    case 'S':
                        return -1;
                    case 'P':
                        return networkHelper.ServerPacketReader.ReadInt32();
                }
            }
            //if (networkHelper.ServerPacketReader.PeekChar() == 'S') return -1;

            //if (networkHelper.ServerPacketReader.PeekChar() == 'P')
            //return networkHelper.ServerPacketReader.ReadInt32();
            return 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IAsyncResult JoinNetwork(Game game,
            NetworkSessionType sessionType, int maxLocalGamers, NetworkSessionProperties sessionProperties)
        {
            SessionManager sessionManager = new SessionManager(game);
            IAsyncResult asyncResult = sessionManager.JoinSession(sessionType, maxLocalGamers, sessionProperties);

            /*if (networkHelper.NetworkGameSession == null)
            {
                return 1;
            }
            return 0;*/
            return asyncResult;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int KillNetwork()
        {
            if (networkHelper.NetworkGameSession != null)
            {
                networkHelper.NetworkGameSession.Dispose();
                networkHelper.NetworkGameSession = null;
            }

            return 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="networkCommand"></param>
        /// <returns></returns>
        // will do this in the future
        public int UpdateNetwork(string networkCommand)
        {
            return 0;
        }

        // send a command to another computer , the command is a string
        public int SyncCommand(string Command)
        {
            if (networkHelper.NetworkGameSession.IsHost)
            {
                networkHelper.ServerPacketWriter.Write('S');
                networkHelper.ServerPacketWriter.Write(Command);
            }
            else
            {
                networkHelper.ClientPacketWriter.Write(Command);
            }
            return 0;
        }

    }
}
