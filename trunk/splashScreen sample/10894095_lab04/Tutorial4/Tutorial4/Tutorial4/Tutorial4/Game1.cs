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

namespace Tutorial4
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        GraphicsDevice device;
        BasicEffect effect;
        SpriteFont font;
        enum gameState { play, menu }; 
        gameState gamestate = gameState.menu;
        MouseState mousetate;
        MouseState prevmousestate;
        List<Menu> menulist = new List<Menu>();
        // Model stuff
        //ModelManager modelManager;
        Texture2D[] skyboxTextures;
        Model skyboxModel;
        Model planeModel;
        Tank tank;
        Matrix rotation;
        Vector3 tankPosition;
        Vector3 targetPosition;
        Vector3 tankDirection;
        // Camera
        public Camera camera { get; protected set; }

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            menulist.Add(new Menu(new Rectangle((Window.ClientBounds.Width / 2) -25, (Window.ClientBounds.Height / 2) -100, 100, 14), "Play"));
            menulist.Add(new Menu(new Rectangle((Window.ClientBounds.Width / 2)-25, (Window.ClientBounds.Height / 2) -50, 100, 14), "Exit"));
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // Initialize Camera
            camera = new Camera(this, new Vector3(135, 13, 43),
                Vector3.Zero, Vector3.Up);
            tankPosition = new Vector3(35, 8.5f, 23);
            //targetPosition = new Vector3(35, 7.5f, 23);
            Components.Add(camera);
            device = graphics.GraphicsDevice;
            tank = new Tank();
            // Initialize model manager
            //modelManager = new ModelManager(this);
            //Components.Add(modelManager);
            this.IsMouseVisible = true;
           
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            font = Content.Load<SpriteFont>("SpriteFont1");
            effect = new BasicEffect(GraphicsDevice);
            planeModel = Content.Load<Model>("Models/Ground");
            skyboxModel = LoadModel("Models/Skybox Model/skybox", out skyboxTextures);
            tank.Load(Content);
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            float time = (float)gameTime.TotalGameTime.TotalSeconds;
            // Update the animation properties on the tank object. In a real game
            // you would probably take this data from user inputs or the physics
            // system, rather than just making everything rotate like this!
            mousetate = Mouse.GetState();

            foreach (Menu m in menulist)
            {
                m.mouseOver(mousetate);
                if (m.text == "Play" && m.isSelected == true && mousetate.LeftButton == ButtonState.Pressed&&prevmousestate.LeftButton==ButtonState.Released) { gamestate = gameState.play; }
            }


            tank.WheelRotation = time * 5;
            tank.SteerRotation = (float)Math.Sin(time * 0.75f) * 0.5f;
            tank.TurretRotation = (float)Math.Sin(time * 0.333f) * 1.25f;
            tank.CannonRotation = (float)Math.Sin(time * 0.25f) * 0.333f - 0.333f;
            tank.HatchRotation = MathHelper.Clamp((float)Math.Sin(time * 2) * 2, -1, 0);
            rotation = Matrix.CreateScale(0.02f) * Matrix.CreateRotationY(time * 0.1f);
            Vector2 mouseLocation = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
            Viewport viewport = this.GraphicsDevice.Viewport;

            if (mousetate.LeftButton == ButtonState.Pressed)
            {
                targetPosition = Move(mouseLocation);
               
                System.Diagnostics.Debug.WriteLine("tankDirection" + tankDirection);
                
            }
            tankDirection = targetPosition - tankPosition;
            tankDirection.Normalize();
            tankPosition += tankDirection  * 0.1f;
            prevmousestate = mousetate;
            //tankPosition += new Vector3(-1, 0, -1);
            //if (Mouse.GetState().LeftButton==)// TODO: Add your update logic here
            System.Diagnostics.Debug.WriteLine(tankPosition+":                     "+targetPosition);
            System.Diagnostics.Debug.WriteLine("?????"+tankDirection);
           // System.Diagnostics.Debug.WriteLine(camera.cameraDirection);
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            float time = (float)gameTime.TotalGameTime.TotalSeconds;
            //Matrix rotation = Matrix.CreateRotationY(time * 0.1f);
            spriteBatch.Begin();
            if (gamestate == gameState.menu)
            {
                device.Clear(Color.Black);
                foreach (Menu m in menulist)
                {
                    m.Draw(spriteBatch, font);
                }
            }
            spriteBatch.End();

            if (gamestate == gameState.play)
            {
                DrawSkybox();
                DrawModel();
                tank.Draw(rotation, camera.view, camera.projection, tankPosition);
            }
            base.Draw(gameTime);
        }

        private Model LoadModel(string assetName, out Texture2D[] textures)
        {

            Model newModel = Content.Load<Model>(assetName);
            textures = new Texture2D[newModel.Meshes.Count];
            int i = 0;
            foreach (ModelMesh mesh in newModel.Meshes)
                foreach (BasicEffect currentEffect in mesh.Effects)
                    textures[i++] = currentEffect.Texture;

            foreach (ModelMesh mesh in newModel.Meshes)
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                    meshPart.Effect = effect.Clone();

            return newModel;
        }

        private void DrawModel()
        {
            Matrix[] transforms = new Matrix[planeModel.Bones.Count];
            planeModel.CopyAbsoluteBoneTransformsTo(transforms);

            foreach (ModelMesh mesh in planeModel.Meshes)
            {
                foreach (BasicEffect be in mesh.Effects)
                {
                    be.EnableDefaultLighting();
                    be.Projection = camera.projection;
                    be.View = camera.view;
                    be.World = Matrix.Identity * mesh.ParentBone.Transform;
                }

                mesh.Draw();
            }
        }

        private void DrawSkybox()
        {
            SamplerState ss = new SamplerState();
            ss.AddressU = TextureAddressMode.Wrap;
            ss.AddressV = TextureAddressMode.Wrap;
            device.SamplerStates[0] = ss;

            DepthStencilState dss = new DepthStencilState();
            dss.DepthBufferEnable = false;
            device.DepthStencilState = dss;

            Matrix[] skyboxTransforms = new Matrix[skyboxModel.Bones.Count];
            skyboxModel.CopyAbsoluteBoneTransformsTo(skyboxTransforms);
            int i = 0;
            foreach (ModelMesh mesh in skyboxModel.Meshes)
            {
                foreach (BasicEffect currentEffect in mesh.Effects)
                {
                    Matrix worldMatrix = skyboxTransforms[mesh.ParentBone.Index] * Matrix.CreateTranslation(camera.cameraPosition);
                    currentEffect.TextureEnabled = true;
                    currentEffect.World = worldMatrix;
                    currentEffect.View = camera.view;
                    currentEffect.Projection = camera.projection;
                    currentEffect.Texture = skyboxTextures[i];
                    i++;
                }
                mesh.Draw();
            }

            dss = new DepthStencilState();
            dss.DepthBufferEnable = true;
            device.DepthStencilState = dss;
        }

      
        private Vector3 Move(Vector2 mouse)
        {
            float mousePositionX = mouse.X;
            float mousePositionY = mouse.Y;
            if (mousePositionX < 0)
                mousePositionX = 0;
            if (mousePositionY < 0)
                mousePositionY = 0;
            if (mousePositionX > graphics.GraphicsDevice.Viewport.Width)
                mousePositionX = (short)graphics.GraphicsDevice.Viewport.Width;
            if (mousePositionY > graphics.GraphicsDevice.Viewport.Height)
                mousePositionY = (short)graphics.GraphicsDevice.Viewport.Height;

            Vector3 nearSource = new Vector3((float)mousePositionX, (float)mousePositionY, 0.0f);
            Vector3 farSource = new Vector3((float)mousePositionX, (float)mousePositionY, 1.0f);
            
            Vector3 nearPoint = graphics.GraphicsDevice.Viewport.Unproject(nearSource, camera.projection, camera.view, Matrix.Identity);
            Vector3 farPoint = graphics.GraphicsDevice.Viewport.Unproject(farSource, camera.projection, camera.view, Matrix.Identity);

            Ray ray = new Ray(nearPoint, Vector3.Normalize(farPoint - nearPoint));
            Plane plane = new Plane(Vector3.Up, -8.5f);

            //modelPosition = ray.Position;

            float denominator = Vector3.Dot(plane.Normal, ray.Direction);
            float numerator = Vector3.Dot(plane.Normal, ray.Position) + plane.D;
            float t = -(numerator / denominator);
            targetPosition = (nearPoint + ray.Direction * t);
            
            System.Diagnostics.Debug.WriteLine("Tank Position  is " + tankPosition + "Ray is " + ray.Position);
            return targetPosition;
        }
    }
}
