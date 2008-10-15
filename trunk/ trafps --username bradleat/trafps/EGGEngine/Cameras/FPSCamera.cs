#region License
//=============================================================================
// System  : Cameras
// File    : FPSCamera.cs
// Author  : Dustin
// Note    : Copyright 2008, Portal Games, All Rights Reserved
// Compiler: Microsoft C#
//
// This file contains the FPS camera.
//
// This code is published under the Microsoft Reciprocal License (Ms-RL). A 
// copy of the license should be distributed with the code. It can also be found
// at the project website: http://www.CodePlex.com/trafps. This notice, the
// author's name, and all copyright notices must remain intact in all
// applications, documentation, and source files.
//
// 
// Todos:   Add input handling for GamePad, find a way to integrate weapon 
//          movement when the player looks around. Or Create a movement system 
//          links to the player, then physics, then updates the camera.
//          The incorprate Weapon movement, etc. Read Bradleat's Topic on
//          Codeplex
//
// ============================================================================ 
#endregion

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using EGGEngine.Helpers;

namespace EGGEngine.Cameras
{
    public class FPSCamera
    {
        Matrix viewMatrix;
        Matrix projectionMatrix;
        Viewport viewPort;

        float leftRightRot;
        float upDownRot;
        const float rotationSpeed = 0.005f;
        Vector3 cameraPosition;
        Vector3 modelPosition;

        Matrix cameraRotation;
        MouseState originalMouseState;
        InputHelper input;

        #region Properties
        public float UpDownRot
        {
            get { return upDownRot; }
            set { upDownRot = value; }
        }

        public float LeftRightRot
        {
            get { return leftRightRot; }
            set { leftRightRot = value; }
        }

        public Matrix ProjectionMatrix
        {
            get { return projectionMatrix; }
        }

        public Matrix ViewMatrix
        {
            get { return viewMatrix; }
        }
        public Vector3 Position
        {
            get { return cameraPosition; }
            set
            {
                cameraPosition = value;
                UpdateViewMatrix();
            }
        }
        public Matrix CameraRotation
        {
            get { return cameraRotation; }
        }
        #endregion

        public FPSCamera(Viewport viewPort)
            : this(viewPort, new Vector3(0, 10, 15), 0, 0)
        {
        }

        public FPSCamera(Viewport viewPort, Vector3 startingPos, float lrRot,
            float udRot)
        {
            this.leftRightRot = lrRot;
            this.upDownRot = udRot;
            this.cameraPosition = startingPos;
            this.viewPort = viewPort;
        
            float viewAngle = MathHelper.PiOver4;
            float nearPlane = 0.5f;
            float farPlane = 10000.0f;
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView
                (viewAngle, viewPort.AspectRatio, nearPlane, farPlane);

            UpdateViewMatrix();

            Mouse.SetPosition(viewPort.Width / 2, viewPort.Height / 2);
            originalMouseState = Mouse.GetState();
            input = new InputHelper();
        }

        /// <summary>
        /// Updates the camera based on the current mouse state.
        /// </summary>
        /// <param name="currentMouseState">The current state of the mouse</param>
        /// <param name="modelPosition">The position of the model in 3D space</param>
        public void Update(MouseState currentMouseState, Vector3 modelPosition)
        {
            this.modelPosition = modelPosition;
            if (currentMouseState != originalMouseState)
            {
                float xDifference = currentMouseState.X - originalMouseState.X;
                float yDifference = currentMouseState.Y - originalMouseState.Y;
                leftRightRot -= rotationSpeed * xDifference;
                upDownRot -= rotationSpeed * yDifference;
                Mouse.SetPosition(viewPort.Width / 2, viewPort.Height / 2);

            }
            UpdateViewMatrix();
        }

        /// <summary>
        /// Helper method that enables moving based on the camera's current
        /// heading.
        /// </summary>
        /// <param name="vectorToAdd">The direction being applied</param>
        /// <param name="position">The position of the model</param>
        public void AddToCameraPosition(Vector3 vectorToAdd, ref Vector3 modelPosition)
        {
            float moveSpeed = 10.0f;
            Vector3 rotatedVector = Vector3.Transform
                (vectorToAdd, cameraRotation);
            modelPosition += moveSpeed * rotatedVector;
            UpdateViewMatrix();
        }

        /// <summary>
        /// Updates the view matrix accordingly.
        /// </summary>
        private void UpdateViewMatrix()
        {
            cameraRotation = Matrix.CreateRotationX(upDownRot) *
                Matrix.CreateRotationY(leftRightRot);

            Vector3 cameraOriginalTarget = new Vector3(0, 0, -1);
            Vector3 cameraOriginalUpVector = new Vector3(0, 1, 0);

            UpdateModelView(cameraRotation);
            
            Vector3 cameraRotatedTarget = Vector3.Transform
                (cameraOriginalTarget, cameraRotation);
            Vector3 cameraFinalTarget = cameraPosition + cameraRotatedTarget;

            Vector3 cameraRotatedUpVector = Vector3.Transform
                (cameraOriginalUpVector, cameraRotation);
            Vector3 cameraFinalUpVector = cameraPosition + cameraRotatedUpVector;        

            viewMatrix = Matrix.CreateLookAt(cameraPosition,
                cameraFinalTarget, cameraRotatedUpVector);            
        }

        /// <summary>
        /// Updates the camera position based on the position and offset
        /// of the model.  Makes it seem like we are viewing from the model's
        /// point of view.
        /// </summary>
        /// <param name="rotation">The current rotation of the camera</param>
        private void UpdateModelView(Matrix rotation)
        {

            //Change the Z value of the offset to 0 for complete first-person mode
            Vector3 avatarHeadOffset = new Vector3(-8, 20, 0);

            Vector3 headOffset = Vector3.Transform(avatarHeadOffset, rotation);

            cameraPosition = modelPosition + headOffset;
        }

    }
}
