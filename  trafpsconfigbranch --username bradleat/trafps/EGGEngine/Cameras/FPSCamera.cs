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
// Revision Number 1
// Sign
// Dustin Heffron, Revision 1
// 
// Todos:   Add input handling for GamePad, find a way to integrate weapon 
//          movement when the player looks around.
//
// ============================================================================ 

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

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
        MouseState originalMouseState;

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
        }

        public void Update(MouseState currentMouseState, KeyboardState keyState)
        {
            if (currentMouseState != originalMouseState)
            {
                float xDifference = currentMouseState.X - originalMouseState.X;
                float yDifference = currentMouseState.Y - originalMouseState.Y;
                leftRightRot -= rotationSpeed * xDifference;
                upDownRot -= rotationSpeed * yDifference;
                Mouse.SetPosition(viewPort.Width / 2, viewPort.Height / 2);
                UpdateViewMatrix();
            }

            if (keyState.IsKeyDown(Keys.Up) || keyState.IsKeyDown(Keys.W))
                AddToCameraPosition(new Vector3(0, 0, -1));
            if (keyState.IsKeyDown(Keys.Down) || keyState.IsKeyDown(Keys.S))
                AddToCameraPosition(new Vector3(0, 0, 1));
            if (keyState.IsKeyDown(Keys.Right) || keyState.IsKeyDown(Keys.D))
                AddToCameraPosition(new Vector3(1, 0, 0));
            if (keyState.IsKeyDown(Keys.Left) || keyState.IsKeyDown(Keys.A))
                AddToCameraPosition(new Vector3(-1, 0, 0));
            if (keyState.IsKeyDown(Keys.Q))
                AddToCameraPosition(new Vector3(0, 1, 0));
            if (keyState.IsKeyDown(Keys.Z))
                AddToCameraPosition(new Vector3(0, -1, 0));
        }

        private void AddToCameraPosition(Vector3 vectorToAdd)
        {
            float moveSpeed = 1.0f;
            Matrix cameraRotation = Matrix.CreateRotationX(upDownRot) *
                Matrix.CreateRotationY(leftRightRot);
            Vector3 rotatedVector = Vector3.Transform
                (vectorToAdd, cameraRotation);
            cameraPosition += moveSpeed * rotatedVector;
            UpdateViewMatrix();
        }

        private void UpdateViewMatrix()
        {
            Matrix cameraRotation = Matrix.CreateRotationX(upDownRot) *
                Matrix.CreateRotationY(leftRightRot);

            Vector3 cameraOriginalTarget = new Vector3(0, 0, -1);
            Vector3 cameraOriginalUpVector = new Vector3(0, 1, 0);

            Vector3 cameraRotatedTarget = Vector3.Transform
                (cameraOriginalTarget, cameraRotation);
            Vector3 cameraFinalTarget = cameraPosition + cameraRotatedTarget;

            Vector3 cameraRotatedUpVector = Vector3.Transform
                (cameraOriginalUpVector, cameraRotation);
            Vector3 cameraFinalUpVector = cameraPosition + cameraRotatedUpVector;

            viewMatrix = Matrix.CreateLookAt(cameraPosition,
                cameraFinalTarget, cameraRotatedUpVector);
        }
    }
}
