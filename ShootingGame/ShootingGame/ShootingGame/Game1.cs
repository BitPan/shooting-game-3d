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
        GameState currentGameState;
        List<GameMenu> gameMenuList;
        GameLevel currentGameLevel;
        LevelData gameLevel;
        MouseState mousetate;
        MouseState prevmousestate;
        public string scoreText;
        private int timeSinceLastShoot;
        private int nextShootTime;
        
        Texture2D[] skyboxTextures;
        Texture2D[] groundTextures;
        Model skyboxModel;
        Model ground;
        Tank tank;
        
        GraphicsDevice device;
        Effect floorEffect;
        Texture2D sceneryTexture;
        Music music;
        Song song;
        SoundEffect soundeffect;
        int finalScore;
        int playerHealth;
        string levelUpText = "";
        int levelUpTextTimer;

        Vector3 tankPosition;
        Matrix rotation;

        private const int boundryLeft = -800;
        private const int boundryRight = 800;
        private const int boundryNear = 1500;
        private const int boundryFar = -1500;    

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            gameMenuList = new List<GameMenu>();
            graphics.PreferredBackBufferWidth = 900;
            graphics.PreferredBackBufferHeight = 500;
            gameLevel = new LevelData();
            currentGameState = GameState.START;
            tank = new Tank();
            this.IsMouseVisible = true;
            timeSinceLastShoot = 0;
            nextShootTime = 0;
            rnd = new Random();
            finalScore = 0;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            camera = new Camera(this, new Vector3(0, 30, 50), new Vector3(0, 30, -1), Vector3.Up);
            modelManager = new ModelManager(this);
            InitializegameMenuList();
            Components.Add(modelManager);
            Components.Add(camera);
            
            base.Initialize();
            // TODO: Add your initialization logic here
        }

        protected void InitializegameMenuList()
        {
            gameMenuList.Add(new GameMenu(new Rectangle((Window.ClientBounds.Width / 2) - 25, (Window.ClientBounds.Height / 2) - 100, 100, 14), "Play"));
            gameMenuList.Add(new GameMenu(new Rectangle((Window.ClientBounds.Width / 2) - 25, (Window.ClientBounds.Height / 2) - 50, 100, 14), "Exit"));
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            device = graphics.GraphicsDevice;

            // Create a new SpriteBatch, which can be used to draw textures.     
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Font1 = Content.Load<SpriteFont>(@"text\SpriteFont1");
            floorEffect = Content.Load<Effect>(@"Effects\effects");
            sceneryTexture = Content.Load<Texture2D>(@"Textures\floortexture");
            skyboxModel = modelManager.LModel(floorEffect, "skybox\\skybox", out skyboxTextures);
            ground = modelManager.LModel(floorEffect, "ground\\Ground", out groundTextures);
    
            song = Content.Load<Song>("music/background");
            soundeffect = Content.Load<SoundEffect>("music/Bomb");
            tank.Load(Content);
            music = new Music(this, song, soundeffect);
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

        private void InitializeGameComponents()
        {
            gameMenuList = new List<GameMenu>();
            modelManager = new ModelManager(this);
            Components.Add(modelManager);
            playerHealth = 100;
            SetNextShootTime(500);
           
            music.BackGroundPlay();

            this.IsMouseVisible = false;
            currentGameState = GameState.PLAY;
            currentGameLevel = GameLevel.LEVEL1;
            modelManager.LoadGameLevelData(gameLevel.loadLevelData(GameLevel.LEVEL1));
            modelManager.addPlayer(camera.cameraPostion, new Vector3());
            FontPos = new Vector2(graphics.GraphicsDevice.Viewport.Width * 0.9f,
            graphics.GraphicsDevice.Viewport.Height * 0.9f);
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
            mousetate = Mouse.GetState();
            timeSinceLastShoot += gameTime.ElapsedGameTime.Milliseconds;
            float fps = (float)(1 / (gameTime.ElapsedGameTime.TotalMilliseconds / 1000));
            float movingDistance = 100 / fps;

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            if (gameMenuList.Count > 0)
            {
                foreach (GameMenu m in gameMenuList)
                {
                    m.mouseOver(mousetate);
                    if (m.isSelected == true &&
                        mousetate.LeftButton == ButtonState.Pressed &&
                        prevmousestate.LeftButton == ButtonState.Released)
                    {
                        if (m.text == "Play")
                            InitializeGameComponents();
                        else if (m.text == "Exit")
                            this.Exit();
                    }
                }
            }
            

            if (currentGameState == GameState.PLAY)
            {
                UpdateTank(gameTime);

                if (playerHealth == 0)
                {
                    currentGameState = GameState.END;
                    InitializegameMenuList();
                    finalScore = modelManager.score;
                    music.BackgroundPause();
                    music.Dispose();
                    modelManager.Dispose();
                    this.IsMouseVisible = true;
                }
                else
                {
                    setGameLevel(modelManager.score);
                    modelManager.spawnEnemy(gameTime);
                    tank.Draw(rotation, camera.view, camera.projection, tankPosition);
                    

                    if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                    {
                        if (timeSinceLastShoot >= nextShootTime)
                        {
                            modelManager.addShot(camera.cameraPostion, camera.cameraDirection);
                            timeSinceLastShoot = 0;
                        }
                    }

                    if (keyState.IsKeyDown(Keys.W))
                        modelManager.GetPlayer().DoTranslation(CalculateTranslation(modelManager.GetPlayer().GetWorld().Translation, new Vector3(movingDistance, 0, movingDistance) * camera.cameraDirection));
                    if (keyState.IsKeyDown(Keys.S))
                        modelManager.GetPlayer().DoTranslation(CalculateTranslation(modelManager.GetPlayer().GetWorld().Translation, new Vector3(-movingDistance, 0, -movingDistance) * camera.cameraDirection));
                    if (keyState.IsKeyDown(Keys.A))
                        modelManager.GetPlayer().DoTranslation(CalculateTranslation(modelManager.GetPlayer().GetWorld().Translation, Vector3.Cross(camera.cameraUp, camera.cameraDirection) * movingDistance));
                    if (keyState.IsKeyDown(Keys.D))
                        modelManager.GetPlayer().DoTranslation(CalculateTranslation(modelManager.GetPlayer().GetWorld().Translation, -Vector3.Cross(camera.cameraUp, camera.cameraDirection) * movingDistance));

                }   
            }

            scoreText = "Health: " + playerHealth + "\nScore:" + modelManager.score + "\nLevel:" + currentGameLevel + "\n: " + (int)camera.cameraPostion.X + "," + (int)camera.cameraPostion.Y + "," + (int)camera.cameraPostion.Z;
            prevmousestate = mousetate;
            base.Update(gameTime);
        }

        protected Vector3 CalculateTranslation(Vector3 playerPosition, Vector3 translation)
        {
            Vector3 preocessedPostion = playerPosition + translation;
            
            preocessedPostion.X = preocessedPostion.X >= boundryRight ? boundryRight : preocessedPostion.X;
            preocessedPostion.X = preocessedPostion.X <= boundryLeft ? boundryLeft : preocessedPostion.X;
            preocessedPostion.Z = preocessedPostion.Z >= boundryNear ? boundryNear : preocessedPostion.Z;
            preocessedPostion.Z = preocessedPostion.Z <= boundryFar ? boundryFar : preocessedPostion.Z;

            Vector3 newTranslation = preocessedPostion - playerPosition;
            return newTranslation;
        }

        protected Boolean setGameLevel(int gameScore)
        {
            GameLevel levelToChange = GameLevel.LEVEL1;

            if (gameScore < 50)
                levelToChange = GameLevel.LEVEL1;
            else if (gameScore >= 50 && gameScore < 100)
                levelToChange = GameLevel.LEVEL2;
            else if (gameScore >= 100 && gameScore < 200)
                levelToChange = GameLevel.LEVEL3;
            else if (gameScore >= 200 && gameScore < 300)
                levelToChange = GameLevel.LEVEL4;
            else if (gameScore >= 300)
                levelToChange = GameLevel.LEVEL5;

            if (levelToChange != currentGameLevel)
            {
                levelUpText = "Level Up, Be Ready!";
                levelUpTextTimer = 2000;
                currentGameLevel = levelToChange;
                modelManager.LoadGameLevelData(gameLevel.loadLevelData(currentGameLevel));
            }
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
            
            if (currentGameState == GameState.START || currentGameState == GameState.END)
            {
                spriteBatch.Begin();
                device.Clear(Color.Black);

                if (currentGameState == GameState.END)
                {
                    FontPos = new Vector2(graphics.GraphicsDevice.Viewport.Width * 0.45f,
                    graphics.GraphicsDevice.Viewport.Height * 0.25f);
                    Vector2 FontOrigin = Font1.MeasureString(scoreText) / 2;
                    // Draw the string
                    spriteBatch.DrawString(Font1, "Your Final Score is: " + finalScore, FontPos, Color.Beige,
                        0, FontOrigin, 1.0f, SpriteEffects.None, 0.5f);
                }
                foreach (GameMenu menu in gameMenuList)
                {
                    menu.Draw(spriteBatch, Font1);
                }
                spriteBatch.End();
            }
            else if (currentGameState == GameState.PLAY)
            {
                modelManager.DrawSkybox(device, camera, skyboxModel, skyboxTextures);
                modelManager.DrawGround(device, camera, ground, groundTextures);
                tank.Draw(rotation, camera.view, camera.projection, tankPosition);

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

                if (levelUpTextTimer > 0)                    
                {
                    levelUpTextTimer -= gameTime.ElapsedGameTime.Milliseconds;
                    Vector2 textSize = Font1.MeasureString(levelUpText);
                    spriteBatch.DrawString(Font1, levelUpText,
                        new Vector2((Window.ClientBounds.Width / 2) - (textSize.X / 2),
                            (Window.ClientBounds.Height / 5) - (textSize.Y / 2)), Color.Goldenrod);
                }
                spriteBatch.End();                
            }

           
            base.Draw(gameTime);
        }

        public void DeductPlayerHealth(int health)
        {
            music.BackgroundPause();
            this.music.EffectPlay();
            playerHealth -= health;
            camera.SetShake(0.2f, 0.4f);
            //music.BackGroundResume();

        }

        private void UpdateTank(GameTime gameTime)
        {
            tankPosition = new Vector3(500, 0, 300);
            float time = (float)gameTime.TotalGameTime.TotalSeconds;
            rotation = rotation = Matrix.CreateScale(0.1f) * Matrix.CreateRotationY(time * 0.1f);
        }

        private void SetNextShootTime(int shootCD)
        {
            nextShootTime = shootCD;
        }
    }
}
