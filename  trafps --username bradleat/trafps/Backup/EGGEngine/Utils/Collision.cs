using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using EGGEngine.Rendering;

namespace EGGEngine.Utils
{
    public static class Collision
    {
        public static bool CoarseCheck(DrawableModel model1, DrawableModel model2)
        {
            BoundingSphere origSphere1 = (BoundingSphere)model1.Model.Tag;
            BoundingSphere sphere1 = XNAUtils.TransformBoundingSphere(origSphere1, model1.WorldMatrix);

            BoundingSphere origSphere2 = (BoundingSphere)model2.Model.Tag;
            BoundingSphere sphere2 = XNAUtils.TransformBoundingSphere(origSphere2, model2.WorldMatrix);

            bool collision = sphere1.Intersects(sphere2);
            return collision;
        }

        public static bool FinerCheck(DrawableModel model1, DrawableModel model2)
        {
           // if (CoarseCheck(model1, model2) == false)
             //   return false;

            bool collision = false;

            foreach (ModelMesh mesh1 in model1.Model.Meshes)
            {
                BoundingSphere origSphere1 = mesh1.BoundingSphere;
                Matrix trans1 = model1.OriginalTransforms[mesh1.ParentBone.Index] * model1.WorldMatrix;
                BoundingSphere transSphere1 =
                    XNAUtils.TransformBoundingSphere(origSphere1, trans1);

                foreach (ModelMesh mesh2 in model2.Model.Meshes)
                {
                    BoundingSphere origSphere2 = mesh2.BoundingSphere;
                    Matrix trans2 = model2.OriginalTransforms[mesh2.ParentBone.Index] * model2.WorldMatrix;
                    BoundingSphere transSphere2 =
                        XNAUtils.TransformBoundingSphere(origSphere2, trans2);

                    if (transSphere1.Intersects(transSphere2))
                        collision = true;
                }
            }

            return collision;
        }

        public static bool FinerCheck2(DrawableModel model1, DrawableModel model2)
        {
            // if (CoarseCheck(model1, model2) == false)
            //   return false;

            bool collision = false;

            foreach (ModelMesh mesh1 in model1.Model.Meshes)
            {
                BoundingSphere origSphere1 = mesh1.BoundingSphere;
                Matrix trans1 = model1.OriginalTransforms[mesh1.ParentBone.Index] * model1.WorldMatrix * Matrix.CreateScale(0.0000000001f);
                BoundingSphere transSphere1 =
                    XNAUtils.TransformBoundingSphere(origSphere1, trans1);

                foreach (ModelMesh mesh2 in model2.Model.Meshes)
                {
                    BoundingSphere origSphere2 = mesh2.BoundingSphere;
                    Matrix trans2 = model2.OriginalTransforms[mesh2.ParentBone.Index] * model2.WorldMatrix * Matrix.CreateScale(0.000000001f);
                    BoundingSphere transSphere2 =
                        XNAUtils.TransformBoundingSphere(origSphere2, trans2);

                    if (transSphere1.Intersects(transSphere2))
                        collision = true;
                }
            }

            return collision;
        }   
    }
}
