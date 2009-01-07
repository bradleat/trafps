#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;
using EGGEngine.Helpers;
using EGGEngine.Cameras;
using EGGEngine.Utils; 
#endregion

namespace EGGEngine.Rendering
{
    public class DrawableModel
    {
        private Model model;
        private Matrix worldMatrix;
        private Matrix[] originalTransforms;
        private Matrix[] modelTransforms;
        public Vector3 position;
        private bool debug = false;
        public float temp = 0f;
        private BoundingSphere completeBoundingSphere;
        public Matrix Rotation;
        public float Life;

        #region Properties
        public Model Model
        {
            get { return model; }
            set { model = value; }
        }
        public Matrix WorldMatrix
        {
            get { return worldMatrix; }
            set { worldMatrix = value; }
        }
        public Matrix[] OriginalTransforms
        {
            get { return originalTransforms; }
            set { originalTransforms = value; }
        }
        public Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }
        #endregion

        public DrawableModel(Model model, Matrix worldMatrix)
        {
            this.model = model;
            this.worldMatrix = worldMatrix;
            modelTransforms = new Matrix[model.Bones.Count];
            
            originalTransforms = new Matrix[model.Bones.Count];
            model.CopyBoneTransformsTo(originalTransforms);

            LoadModelBoundingSphere();

            position = new Vector3();
            
            if (debug)
                WriteModelStructure(model);
        }

        public void EnemyRecieveDamage(int damageValue)
        {
            this.Life -= damageValue;
            if (Life < 1)
            {
                //respawn
                Random random = new Random();
                this.position.X = (float)random.NextDouble() * 100;
            }
        }
        public void PlayerRecieveDamage(int damageValue)
        {
            this.Life -= damageValue;
            if (Life < 1)
            {
                //respawn
                this.position = new Vector3(0, 15, 0);
                Life = 100;
            }
        }
        /// <summary>
        /// Draws the model in 3D space using the current camera's view matrix 
        /// and projection matrix.
        /// </summary>
        /// <param name="camera">The current camera being used</param>
        public void Draw(FPSCamera camera)
        {
            InputHelper input = new InputHelper();
            if (input.KeyDown(Keys.F))
                temp += 0.05f;

            model.CopyAbsoluteBoneTransformsTo(modelTransforms);
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World = worldMatrix * modelTransforms[mesh.ParentBone.Index] 
                        * Matrix.CreateTranslation(position);
                    effect.View = camera.ViewMatrix;
                    effect.Projection = camera.ProjectionMatrix;
                }
                mesh.Draw();
            }
        }

        /// <summary>
        /// Loads the bounding sphere of the model into it's tag.
        /// </summary>
        private void LoadModelBoundingSphere()
        {
             completeBoundingSphere = new BoundingSphere();

            foreach (ModelMesh mesh in model.Meshes)
            {
                BoundingSphere origMeshSphere = mesh.BoundingSphere;
                BoundingSphere transMeshSphere = XNAUtils.TransformBoundingSphere(origMeshSphere,
                    originalTransforms[mesh.ParentBone.Index]);
                completeBoundingSphere =
                    BoundingSphere.CreateMerged(completeBoundingSphere, transMeshSphere);
            }
            
            model.Tag = completeBoundingSphere.Transform(Matrix.CreateScale(0.5f));
        }

        /// <summary>
        /// Debugging method that writes the structure of the model into
        /// a text file, including bone names, mesh names, and indices of each.
        /// </summary>
        /// <param name="model">The model being used</param>
        private void WriteModelStructure(Model model)
        {
            StreamWriter writer = new StreamWriter(model.Root.Name + "modelStructure.txt");

            writer.WriteLine("Model Bone Information");
            writer.WriteLine("----------------------");

            ModelBone root = model.Root;
            WriteBone(root, 0, writer);

            writer.WriteLine();
            writer.WriteLine();

            writer.WriteLine("Model Mesh Information");
            writer.WriteLine("----------------------");

            foreach (ModelMesh mesh in model.Meshes)
                WriteModelMesh(model.Meshes.IndexOf(mesh), mesh, writer);

            writer.Close();
        }

        /// <summary>
        /// Debugging method that handles the writing of the bone structure.
        /// </summary>
        /// <param name="bone">The current bone</param>
        /// <param name="level">The level in the bone structure (number of tabs)</param>
        /// <param name="writer">The stream writer used to write the information
        /// to a text file</param>
        private void WriteBone(ModelBone bone, int level, StreamWriter writer)
        {
            for (int i = 0; i < level; ++i)
                writer.Write("\t");
            writer.Write("- Name: ");
            if ((bone.Name == "") || (bone.Name == "null"))
                writer.WriteLine("null");
            else
                writer.WriteLine(bone.Name);

            for (int i = 0; i < level; ++i)
                writer.Write("\t");
            writer.WriteLine("  Index: " + bone.Index);

            foreach (ModelBone childBone in bone.Children)
                WriteBone(childBone, level + 1, writer);
        }

        /// <summary>
        /// Debugging method that handles the writing of the mesh structure.
        /// </summary>
        /// <param name="ID">The index of the current mesh</param>
        /// <param name="mesh">The current mesh</param>
        /// <param name="writer">The stream writer used to write the information
        /// to a text file</param>
        private void WriteModelMesh(int ID, ModelMesh mesh, StreamWriter writer)
        {
            writer.WriteLine("- ID : " + ID);
            writer.WriteLine(" Name: " + mesh.Name);
            writer.Write(" Bone: " + mesh.ParentBone.Name);
            writer.WriteLine(" (" + mesh.ParentBone.Index + ")");
        }
    }
}
