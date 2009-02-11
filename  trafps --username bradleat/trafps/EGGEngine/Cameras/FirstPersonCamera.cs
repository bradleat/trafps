using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using EGGEngine.Helpers;
using EGGEngine.Rendering;

namespace EGGEngine.Cameras
{
    public class FirstPersonCamera
    {
        private Vector3 _position;
        private Vector3 _lookAt;
        private Matrix _viewMatrix;
        private Matrix _projectionMatrix;
        private float _aspectRatio;
        private float _upDownRot;
        //private float leftrightRot = MathHelper.PiOver2;
        //private float updownRot = -MathHelper.Pi / 10.0f;
        //private const float rotationSpeed = 0.3f;

        private MouseState originalMouseState;
        private Viewport viewport;
        private float _modelRotation;


        public FirstPersonCamera(Viewport viewport)
        {
            this._aspectRatio = ((float)viewport.Width) / ((float)viewport.Height);
            this._projectionMatrix = Matrix.CreatePerspectiveFieldOfView(
                                        MathHelper.ToRadians(45.0f),
                                        1.0f,
                                        1.0f,
                                        10000.0f);
            Mouse.SetPosition(viewport.Width / 2, viewport.Height / 2);
            originalMouseState = Mouse.GetState();
            this.viewport = viewport;
        }

        public Vector3 Position
        {
            get { return this._position; }
            set { this._position = value; }
        }
        public Vector3 LookAt
        {
            get { return this._lookAt; }
            set { this._lookAt = value; }
        }
        public Matrix ViewMatrix
        {
            get { return this._viewMatrix; }
        }
        public Matrix ProjectionMatrix
        {
            get { return this._projectionMatrix; }
        }
        public float UpdownRot
        {
            get { return this._upDownRot; }
            set { this._upDownRot = value; }
        }
        
        public void Update(float modelRotation)
        {
            this._modelRotation = modelRotation;

            UpdateViewMatrix(modelRotation);
            
        }

        private void UpdateViewMatrix(float modelRotation)
        {
            Matrix cameraRotation = Matrix.CreateRotationX(UpdownRot) * Matrix.CreateRotationY(modelRotation);// *MathHelper.ToRadians(180);
            Vector3 cameraOriginalTarget = new Vector3(0, 0, 1);                  
            Vector3 cameraRotatedTarget = Vector3.Transform(cameraOriginalTarget, cameraRotation);
            
            Vector3 cameraFinalTarget = this.Position + cameraRotatedTarget;

            Vector3 cameraOriginalUpVector = new Vector3(0, 1, 0);                    
            Vector3 cameraRotatedUpVector = Vector3.Transform(cameraOriginalUpVector, cameraRotation);

            this._viewMatrix  = Matrix.CreateLookAt(this._position, cameraFinalTarget, cameraRotatedUpVector);
        }

        public Matrix WeaponWorldMatrix(float xOffset, float yOffset, float zOffset, float scale)
        {
            Vector3 weaponPos = _position;

            weaponPos += Vector3.Forward * zOffset;
            weaponPos += new Vector3(0,1,0) * yOffset;
            weaponPos += new Vector3(1, 0, 0) * xOffset;

            Matrix matrix = _viewMatrix;
            
            

            return Matrix.CreateScale(scale)* Matrix.CreateRotationX(_modelRotation)
                    * Matrix.CreateRotationY(UpdownRot)
                    * Matrix.CreateTranslation(weaponPos);
        }
        public Matrix WeaponWorldMatrix(Vector3 Position, float updown, float leftright, Vector3 forwardVector, Model model)
        {

            Vector3 xAxis;
            Vector3 yAxis;

            // Copy any parent transforms.
            Matrix[] transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);

            xAxis.X = _viewMatrix.M11;
            xAxis.Y = _viewMatrix.M21;
            xAxis.Z = _viewMatrix.M31;

            yAxis.X = _viewMatrix.M12;
            yAxis.Y = _viewMatrix.M22;
            yAxis.Z = _viewMatrix.M32;

            Position += forwardVector; // 5;  //How far infront of the camera The gun will be
            Position += xAxis * 0.1f;      //X axis offset
            Position += -yAxis * 6f;     //Y axis offset

            return transforms[model.Meshes.Count]
                * 
                Matrix.CreateScale(0.5f) *                        //Size of the Gun 
                Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians(5), 0, 0)      //Rotation offset
                * Matrix.CreateRotationX(updown)
                * Matrix.CreateRotationY(leftright)
                * Matrix.CreateTranslation(Position);
        }
    }


}
