#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;
using EGGEngine.Helpers;
using EGGEngine.Cameras;
#endregion

namespace EGGEngine.Rendering
{
    public class DrawableModel
    {
        private Model model;
        private Matrix worldMatrix;
        private Matrix[] originalTransforms;
        private Matrix[] modelTransforms;
        private Vector3 position;
        private bool debug = false;
        public float temp = 0f;

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

            position = new Vector3();

            if (debug)
                WriteModelStructure(model);
        }

        public void Draw(FPSCamera camera)
        {
            InputHelper input = new InputHelper();
            if (input.KeyDown(Keys.F))
                temp += 0.05f;

            model.Bones[0].Transform = //Matrix.CreateRotationX(camera.UpDownRot) *
                originalTransforms[0] * Matrix.CreateRotationX(camera.UpDownRot)
                * Matrix.CreateRotationY(camera.LeftRightRot);


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

        private void WriteModelMesh(int ID, ModelMesh mesh, StreamWriter writer)
        {
            writer.WriteLine("- ID : " + ID);
            writer.WriteLine(" Name: " + mesh.Name);
            writer.Write(" Bone: " + mesh.ParentBone.Name);
            writer.WriteLine(" (" + mesh.ParentBone.Index + ")");
        }
    }
}
