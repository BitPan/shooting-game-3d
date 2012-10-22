using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace ShootingGame
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class BackGround : Microsoft.Xna.Framework.GameComponent
    {
        Effect floorEffect;

        public BackGround(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here
            floorEffect = Game.Content.Load<Effect>("effects");
            base.Initialize();
        }



        public Model LModel(string assetName, out Texture2D[] textures)
        {

            Model newModel = this.Game.Content.Load<Model>(@assetName);
            textures = new Texture2D[newModel.Meshes.Count];
            int i = 0;
            foreach (ModelMesh mesh in newModel.Meshes)
                foreach (BasicEffect currentEffect in mesh.Effects)
                    textures[i++] = currentEffect.Texture;

            foreach (ModelMesh mesh in newModel.Meshes)
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                    meshPart.Effect = floorEffect.Clone();

            return newModel;
        }

        public void DrawGround(GraphicsDevice device, FirstPersonCamera camera, Model ground, Texture2D[] groundTextures)
        {
            SamplerState ss = new SamplerState();
            ss.AddressU = TextureAddressMode.Clamp;
            ss.AddressV = TextureAddressMode.Clamp;
            device.SamplerStates[0] = ss;

            DepthStencilState dss = new DepthStencilState();
            dss.DepthBufferEnable = false;
            device.DepthStencilState = dss;

            Matrix[] groundTransforms = new Matrix[ground.Bones.Count];
            ground.CopyAbsoluteBoneTransformsTo(groundTransforms);
            int i = 0;
            foreach (ModelMesh mesh in ground.Meshes)
            {
                foreach (Effect currentEffect in mesh.Effects)
                {
                    Matrix worldMatrix = groundTransforms[mesh.ParentBone.Index] * Matrix.CreateTranslation(new Vector3(0, 0, 0));
                    currentEffect.CurrentTechnique = currentEffect.Techniques["Textured"];
                    currentEffect.Parameters["xWorld"].SetValue(worldMatrix);
                    currentEffect.Parameters["xView"].SetValue(camera.ViewMatrix);
                    currentEffect.Parameters["xProjection"].SetValue(camera.ProjectionMatrix);
                    currentEffect.Parameters["xTexture"].SetValue(groundTextures[i++]);
                }
                mesh.Draw();
            }

            dss = new DepthStencilState();
            dss.DepthBufferEnable = true;
            device.DepthStencilState = dss;
        }

        

        public void DrawSkybox(GraphicsDevice device, FirstPersonCamera camera, Model skyboxModel, Texture2D[] skyboxTextures)
        {
            SamplerState ss = new SamplerState();
            ss.AddressU = TextureAddressMode.Clamp;
            ss.AddressV = TextureAddressMode.Clamp;
            device.SamplerStates[0] = ss;

            DepthStencilState dss = new DepthStencilState();
            dss.DepthBufferEnable = false;
            device.DepthStencilState = dss;
            Matrix[] skyboxTransforms = new Matrix[skyboxModel.Bones.Count];
            skyboxModel.CopyAbsoluteBoneTransformsTo(skyboxTransforms);
            int i = 0;
            foreach (ModelMesh mesh in skyboxModel.Meshes)
            {
                foreach (Effect currentEffect in mesh.Effects)
                {
                    Matrix worldMatrix = Matrix.CreateScale(200) * skyboxTransforms[mesh.ParentBone.Index] * Matrix.CreateTranslation(camera.Position - (new Vector3(0, 50, 0)));
                    currentEffect.CurrentTechnique = currentEffect.Techniques["Textured"];
                    currentEffect.Parameters["xWorld"].SetValue(worldMatrix);
                    currentEffect.Parameters["xView"].SetValue(camera.ViewMatrix);
                    currentEffect.Parameters["xProjection"].SetValue(camera.ProjectionMatrix);
                    currentEffect.Parameters["xTexture"].SetValue(skyboxTextures[i++]);
                }
                mesh.Draw();
            }

            dss = new DepthStencilState();
            dss.DepthBufferEnable = true;
            device.DepthStencilState = dss;
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here

            base.Update(gameTime);
        }
    }
}
