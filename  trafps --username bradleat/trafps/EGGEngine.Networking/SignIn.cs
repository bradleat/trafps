#region License
//=============================================================================
// System  : Networking
// File    : SignIn.cs
// Author  : EV and Bradley Leatherwood
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
#region RevisionCount
/// <revisioncount>
/// Revision Number (add one each time you edit code) 1
/// Sign:
/// Bradley Leatherwood, Revision 1
/// Bradley Leatherwood, Revision 2
/// </revisioncount>
#endregion
// 
// Todos: Design
//
// ============================================================================
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.GamerServices;
#endregion
//ALWAYS ADD YOUR SIGNATURE TO THE REVISIONS IN THE LICENSE> REVISION REGION

namespace EGGEngine.Networking
{
    #region Summary
    /// <summary>
    /// This Class Signs a Player in and checks to see if the player is already signed in.
    /// </summary>
    #endregion
    #region Todos
    /// <todo>
    /// CHECK THIS AGAINST THE UML, REDESIGN IF NEEDED, THIS IS SAMPLE ONLY
    /// (THERE IS NO CODE IN MOST OF THE FUNCTIONS IN THIS FILE, PLEASE FILL THEM)
    /// 1. Make sure the class wont launch the sign-in guide if a sign-guide is already showing
    /// 2. Store the Player's Gamertag in a String
    /// 3. Check to See if the Player is Live Enabled if not then display a message
    /// 4. Check to see if the player is already signed in.
    /// 5. Make A sample Game to Sign the Player in and test furture Networking Features
    /// </todo> 
    #endregion


    public class SignIn :Microsoft.Xna.Framework.Net
    {
        #region Vars
        private bool SignedIn; //shows if the player is signed in
        private bool GuideShowing; //shows if the guide is showing
        private bool LiveEnabled; // shows if xbox live is enabled for the profile
        private int paneCount = 1; // Sets the Number of Panes to display at startup
        private bool onlineOnly = true; // If Set to True, the Guide Only Displays Online ONLY Profiles when signing in
        private string PlayerGamerTag; //stores the player's gamer tag
        #endregion



        public static void ShowSignIn(int paneCount, bool onlineOnly);

        #region GuideShowingFunction
        public bool GuideShowing()
        {
            return GuideShowing;
        }
        #endregion

        #region AlreadySignedInFunction
        private bool AleadySignedIn(string gamerTag)
        {
            return SignedIn;
        }
        #endregion

        #region LiveEnabledFunction
        private bool LiveEnabled(gamerTag)
        {
            return LiveEnabled;
        }
        #endregion

        #region SignIn
        public SignIn()
        {
            if (AlreadySignedIn = true)
            {
                return;
            }
            else
            {
                ShowSignIn(paneCount, onlineOnly);
            }
        }
        #endregion

    }
}
