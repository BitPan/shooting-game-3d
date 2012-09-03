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
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        public Camera camera {get; protected set;}
        SpriteFont Font1;
        Vector2 FontPos;
        BasicEffect effect;
        ModelManager modelManager;
        public Random rnd { get; protected set; }
        public enum GameState { START, PLAY, END };
        public enum GameLevel { LEVEL1,LEVEL2,LEVEL3,LEVEL4,LEVEL5};
        GameState currentGameState = GameState.PLAY;
        GameLevel currentGameLevel;
        LevelData gameLevel;
        public string scoreText;
        private int timeSinceLastShoot = 0;
        private int nextShootTime = 0;
        private static int CITY_WIDTH = 500;
        private static int CITY_LENGTH = 500;

        private const float WEAPON_SCALE = 0.007f;
        private const float WEAPON_X_OFFSET = 0;
        private const float WEAPON_Y_OFFSET = 0;
        private const float WEAPON_Z_OFFSET = 80f;
        Texture2D[] skyboxTextures;
        Texture2D[] groundTextures;
        Model skyboxModel;
        Model ground;
        Model weapon;

        private Matrix[] weaponTransforms;
        private Matrix weaponWorldMatrix;
        
        private int[,] groundPlan;
        GraphicsDevice device;
        Effect floorEffect;
        Texture2D sceneryTexture;
        VertexBuffer cityVertexBuffer;


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = 900;
            graphics.PreferredBackBufferHeight = 500;
            gameLevel = new LevelData();
            rnd = new Random();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            camera = new Camera(this, new Vector3(0, 30, 50), Vector3.Zero, Vector3.Up);
            modelManager = new ModelManager(this);
            Components.Add(modelManager);
            Components.Add(camera);
            currentGameLevel = GameLevel.LEVEL1;
            
            SetNextShootTime();

            
            base.Initialize();
            // TODO: Add your initialization logic here
            
       
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Font1 = Content.Load<SpriteFont>(@"text\SpriteFont1");

            FontPos = new Vector2(graphics.GraphicsDevice.Viewport.Width *0.9f,
            graphics.GraphicsDevice.Viewport.Height * 0.9f);
            effect = new BasicEffect(GraphicsDevice);
            //modelManager.addPlayer(new Vector3(0, 0, 0), new Vector3());
            modelManager.addPlayer(camera.cameraPostion, new Vector3());
            device = graphics.GraphicsDevice;
            floorEffect = Content.Load<Effect>(@"Effects\effects");
            sceneryTexture = Content.Load<Texture2D>(@"Textures\floortexture");
            //setUpGround();

            skyboxModel = modelManager.LModel(floorEffect, "skybox\\skybox", out skyboxTextures);
            ground = modelManager.LModel(floorEffect, "ground\\Ground", out groundTextures);
            weapon = Content.Load<Model>(@"Models\weapon");

            RasterizerState rs = new RasterizerState();
            rs.CullMode = CullMode.CullCounterClockwiseFace;
            GraphicsDevice.RasterizerState = rs;

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
            timeSinceLastShoot += gameTime.ElapsedGameTime.Milliseconds; 
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();


            if (currentGameState == GameState.PLAY)
            {
                setGameLevel(modelManager.score);
                modelManager.spawnEnemy(gameTime);
                if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                {
                    if (timeSinceLastShoot >= nextShootTime)
                    {
                        modelManager.addShot(camera.cameraPostion, camera.cameraDirection);
                        timeSinceLastShoot = 0;
                    }
                }               
            }

          
            
            //scoreText = modelManager.textToDisplay1 + "\n" + modelManager.textToDisplay2 + "\n" + modelManager.textToDisplay3 + "\n";
            scoreText = "Health: " + modelManager.playerHealth + "\nScore:" + modelManager.score + "\nLevel:" + currentGameLevel;
            base.Update(gameTime);
        }

        protected Boolean setGameLevel(int gameScore)
        {
            GameLevel levelToChange = GameLevel.LEVEL1;

            if (gameScore < 100)
                levelToChange = GameLevel.LEVEL1;
            else if (gameScore >= 100 && gameScore < 300)
                levelToChange = GameLevel.LEVEL2;
            else if (gameScore >= 300 && gameScore < 600)
                levelToChange = GameLevel.LEVEL3;
            else if (gameScore >= 600 && gameScore < 1000)
                levelToChange = GameLevel.LEVEL4;
            else if (gameScore >= 1000 && gameScore < 1500)
                levelToChange = GameLevel.LEVEL5;

            if (levelToChange != currentGameLevel)
                currentGameLevel = levelToChange; 
            modelManager.LoadGameLevelData(gameLevel.loadLevelData(currentGameLevel));
            return true;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            //DrawCity();
            modelManager.DrawSkybox(device, camera, skyboxModel, skyboxTextures);
            modelManager.DrawGround(device, camera, ground, groundTextures);
           
            // TODO: Add your drawing code here
            effect.World = Matrix.Identity;
            effect.View = camera.view;
            effect.Projection = camera.projection;
            spriteBatch.Begin();
            // Find the center of the string
            Vector2 FontOrigin = Font1.MeasureString(scoreText) / 2;
            // Draw the string
            spriteBatch.DrawString(Font1, scoreText, FontPos, Color.LightGreen,
                0, FontOrigin, 1.0f, SpriteEffects.None, 0.5f);

            spriteBatch.End();

           
            base.Draw(gameTime);
        }


    

        private void SetNextShootTime()
        {
            nextShootTime = 500;
        }
    }
}
