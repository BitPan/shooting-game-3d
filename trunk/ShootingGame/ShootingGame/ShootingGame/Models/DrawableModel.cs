using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ShootingGame.Models
{
    public class DrawableModel : IModelBase
    {
        protected Matrix worldMatrix;
        protected Model model;
        protected Matrix[] modelTransforms;
        protected Vector3 position;
        protected int modelID;
        protected Vector3 direction;
        protected Matrix rotation = Matrix.Identity;

        public Matrix WorldMatrix
        {
            get { return worldMatrix; }
            set
            {
                worldMatrix = value;
                position = new Vector3(value.M41, value.M42, value.M43);
            }
        }

        public Vector3 Position { get { return position; } set { position = value; } }
        public Vector3 Direction { get { return direction; } }
        public Model Model { get { return model; } }
        public int ModelID { get { return modelID; } set { modelID= value; } }

        public DrawableModel(Model inModel, Matrix inWorldMatrix, Vector3 direction)
        {
            this.direction = direction;
            model = inModel;
            modelTransforms = new Matrix[model.Bones.Count];
            worldMatrix = inWorldMatrix;
            position = new Vector3(inWorldMatrix.M41, inWorldMatrix.M42, inWorldMatrix.M43);
            position = inWorldMatrix.Translation;
        }

        public virtual void Draw(Matrix viewMatrix, Matrix projectionMatrix)
        {
            model.CopyAbsoluteBoneTransformsTo(modelTransforms);
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World = modelTransforms[mesh.ParentBone.Index] * worldMatrix * rotation;
                    effect.View = viewMatrix;
                    effect.Projection = projectionMatrix;
                }
                mesh.Draw();
            }
        }

        public bool CollidesWith(Model otherModel, Matrix otherWorld)
        {
            foreach (ModelMesh myModelMeshes in model.Meshes)
            {
                foreach (ModelMesh hisModelMeshes in otherModel.Meshes)
                {
                    if (myModelMeshes.BoundingSphere.Transform(worldMatrix).Intersects(
                        hisModelMeshes.BoundingSphere.Transform(otherWorld)))
                        return true;
                }
            }
            return false;
        }       

        public virtual void Update()
        {
        }
    }
}
