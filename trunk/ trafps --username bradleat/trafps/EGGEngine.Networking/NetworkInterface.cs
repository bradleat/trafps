﻿#region License
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
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.GamerServices;
#endregion

namespace EGGEngine.Networking
{
    class NetworkInterface
    {
        NetworkHelper networkHelper;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        int InitNetwork()
        {
            return 5;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        int CreateNetwork()
        {
            return 5;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        int FrameNetwork()
        {
            return 5;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        int JoinNetwork()
        {
            return 5;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        int KillNetwork()
        {
            return 5;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="networkCommand"></param>
        /// <returns></returns>
        int UpdateStatus(string networkCommand)
        {
            return 5;
        }
    }
}