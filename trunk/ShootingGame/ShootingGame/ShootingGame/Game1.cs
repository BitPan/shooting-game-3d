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
using ShootingGame.Core;
using ShootingGame.GameComponent;

namespace ShootingGame
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        public FirstPersonCamera camera { get; protected set; }
        SpriteFont Font1;
        Vector2 FontPos;
        BasicEffect effect;
        public Random rnd { get; protected set; }
        List<GameMenu> gameMenuList;

        Texture2D[] skyboxTextures;
        Model skyboxModel;
        Model myModel, weapon;

        GraphicsDevice device;
        Effect floorEffect;
        Music music;

        BackGround background;
        Texture2D sceneryTexture;
        SceneManager sceneManager;
        InputHandler inputHandler;
        TextHandler textHandler;



        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            gameMenuList = new List<GameMenu>();
            graphics.PreferredBackBufferWidth = 900;
            graphics.PreferredBackBufferHeight = 500;
            this.IsMouseVisible = true;
            camera = new FirstPersonCamera(this);

            Components.Add(camera);
            inputHandler = new InputHandler();
            textHandler = new TextHandler();

        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            background = new BackGround(this);
            sceneManager = new SceneManager(this);
            Components.Add(sceneManager);
            camera.prepareCamera();
            base.Initialize();
            InitializeGameComponents();


            // TODO: Add your initialization logic here
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            device = graphics.GraphicsDevice;

            spriteBatch = new SpriteBatch(GraphicsDevice);
            Font1 = Content.Load<SpriteFont>(@"text\SpriteFont1");
            floorEffect = Content.Load<Effect>("effects");
            sceneryTexture = Content.Load<Texture2D>("texturemap");
            music = new Music(this);

            RasterizerState rs = new RasterizerState();
            rs.CullMode = CullMode.CullCounterClockwiseFace;
            GraphicsDevice.RasterizerState = rs;
            FontPos = new Vector2(graphics.GraphicsDevice.Viewport.Width * 0.9f,
                    graphics.GraphicsDevice.Viewport.Height * 0.9f);
            skyboxModel = background.LModel(floorEffect, "skybox\\skybox", out skyboxTextures);
            weapon = Content.Load<Model>(@"Models\weapon");
            myModel = Content.Load<Model>(@"Models\Ship");
            camera.setWeapon(weapon);
            sceneManager.GetCity().SetUpCity(device, sceneryTexture);

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        public void InitializeGameComponents()
        {
            gameMenuList = new List<GameMenu>();
            music.BackGroundPlay();

            this.IsMouseVisible = false;
            effect = new BasicEffect(GraphicsDevice);
        }



        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            KeyboardState keyState = Keyboard.GetState();
            //Press Exit to exit the game
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || keyState.IsKeyDown(Keys.Escape))
                this.Exit();
            inputHandler.UpdateWorld(gameTime, camera, sceneManager, music);
            textHandler.UpdateText(sceneManager);
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            /*
            if (currentGameState == GameState.START || currentGameState == GameState.END)
            {
                spriteBatch.Begin();
                device.Clear(Color.Black);

                if (currentGameState == GameState.END)
                {
                }

                foreach (GameMenu menu in gameMenuList)
                {
                    menu.Draw(spriteBatch, Font1);
                }
                spriteBatch.End();
            }
            else if (currentGameState == GameState.PLAY)
            {
            */
            sceneManager.GetOcTreeRoot.ModelsDrawn = 0;

            BoundingFrustum cameraFrustrum = new BoundingFrustum(camera.ViewMatrix * camera.ProjectionMatrix);
            sceneManager.GetOcTreeRoot.Draw(camera.ViewMatrix, camera.ProjectionMatrix, cameraFrustrum);
            sceneManager.GetOcTreeRoot.DrawBoxLines(camera.ViewMatrix, camera.ProjectionMatrix, device, effect);
            Window.Title = string.Format("Models drawn: {0}", sceneManager.GetOcTreeRoot.ModelsDrawn);
            sceneManager.GetCity().DrawCity(device, camera, floorEffect, 50f, 0f, new Vector3(0, 0, 0));
            camera.DrawWeapon();
            textHandler.DrawText(Font1, spriteBatch, gameTime, FontPos);

            base.Draw(gameTime);
        }

        public FirstPersonCamera GetGameCamera()
        {
            return this.camera;
        }

        public void PlayBackGroundMusic()
        {

            this.music.BackGroundPlay();

        }

        public void BackGroundPause()
        {
            this.music.BackgroundPause();
        }

        public void BackGroudResumePlay()
        {
            this.music.BackGroundResume();

        }
    }
}
