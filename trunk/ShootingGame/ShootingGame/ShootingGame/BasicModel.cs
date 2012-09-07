using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ShootingGame
{
    public class BasicModel
    {

        public Model model { get; protected set; }
        public Matrix world = Matrix.Identity;//控制model的绘制位置，旋转缩放等
        public Vector3 direction {get; protected set;}


        public BasicModel(Model m)
        {
            model = m;
        }

        public virtual Matrix GetWorld()
        {
            return world;
        }

        public bool CollidesWith(Model otherModel, Matrix otherWorld)
        {
            foreach (ModelMesh myModelMeshes in model.Meshes)
            {
                foreach (ModelMesh hisModelMeshes in otherModel.Meshes)
                {
                    if (myModelMeshes.BoundingSphere.Transform(GetWorld()).Intersects(
                        hisModelMeshes.BoundingSphere.Transform(otherWorld)))
                        return true;
                }
            }
            return false;
        }

        public virtual void Update()
        {
        }

        public void Draw(Camera camera)
        {
            Matrix[] transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World = GetWorld() * mesh.ParentBone.Transform;
                    effect.View = camera.view;
                    effect.Projection = camera.projection;
                }
                mesh.Draw();
            }
        }
    }
}
