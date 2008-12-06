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
    class NetworkHelper
    {
        PacketWriter serverPacketWriter;
        PacketWriter clientPacketWriter;
        PacketReader serverPacketReader;
        PacketReader clientPacketReader;

        NetworkSession networkSession;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        int SendSvData()
        {
            return 5;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        int SendClData()
        {
            return 5;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        int ReadSvData()
        {
            return 5;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        int ReadClData()
        {
            return 5;
        }
    }
}
