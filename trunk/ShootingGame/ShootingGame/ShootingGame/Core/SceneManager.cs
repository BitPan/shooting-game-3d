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
using ShootingGame.Models;
using ShootingGame.Data;
using ShootingGame.GameComponent;


namespace ShootingGame.Core
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class SceneManager : DrawableGameComponent
    {
        Game game;
        City city;        
        Octree octreeWorld;
        Music music;
        BackGround background;

        GameWorldData worldData;       
        GameLevelHandler levelHander;
        InputHandler inputHandler;
        TextHandler textHandler;
        GameMenuScreen gameMenu;

        BasicEffect effect;
        Effect floorEffect;
        Texture2D sceneryTexture;

        public FirstPersonCamera camera { get; protected set; }

        private int playerHealth;
        private int playerScore;
        
        
        public int GetPlayerHealth { get { return playerHealth; } }
        public OcTreeNode GetOcTreeRoot { get { return octreeWorld.GetOctree(); } }

        public SceneManager(Game game)
            : base(game)
        {
            this.game = game;
            city = new City();
            inputHandler = new InputHandler();
            textHandler = new TextHandler();
            levelHander = new GameLevelHandler();
            gameMenu = new GameMenuScreen(game, levelHander);            
            worldData = new GameWorldData();
            camera = new FirstPersonCamera(game);
            music = new Music(game);
            background = new BackGround(game);
            camera.prepareCamera();
            camera.setWeapon(Game.Content.Load<Model>(@"Models\weapon"));
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {           
            playerHealth = 100;
            playerScore = 0;
            Game.Components.Add(gameMenu);
            effect = new BasicEffect(Game.GraphicsDevice);
            sceneryTexture = Game.Content.Load<Texture2D>("texturemap");
            floorEffect = Game.Content.Load<Effect>("effects");
            base.Initialize();
        }


        public void AddPlayerBulletModel(Vector3 position, Vector3 direction)
        {
            octreeWorld.AddPlayerBulletModel(position, direction);
        }        

        private void LoadWorldModels()
        {
            List<Model> models = new List<Model>();
            models.Add(Game.Content.Load<Model>("Models\\spaceship"));
            models.Add(Game.Content.Load<Model>("Models\\ship"));
            models.Add(Game.Content.Load<Model>("Models\\ammo"));
            models.Add(Game.Content.Load<Model>("Models\\ammo"));
            models.Add(Game.Content.Load<Model>("Models\\tank"));
            octreeWorld.LoadModels(models);
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            float time = (float)gameTime.TotalGameTime.TotalMilliseconds / 1000.0f;
            MouseState mouseState = Mouse.GetState();
            KeyboardState keyState = Keyboard.GetState();
            levelHander.UpdateGameStatus(playerScore);

            if (levelHander.GetGameState == GameLevelHandler.GameState.INITIALIZE)
            {
                octreeWorld = new Octree(this, worldData);
                LoadWorldModels();
                octreeWorld.TestInitialize(levelHander.GetEmemyData);
                levelHander.SetGameState = GameLevelHandler.GameState.PLAY;
                Game.Components.Remove(gameMenu);
                game.Components.Add(camera);
                music.BackGroundPlay();
                game.IsMouseVisible = false;
                textHandler.UpdateText(this);
                city.SetUpCity(Game.GraphicsDevice, sceneryTexture);
            }
            else if (levelHander.GetGameState == GameLevelHandler.GameState.PLAY)
            {
                textHandler.UpdateText(this);
                inputHandler.UpdateWorld(gameTime, camera, this, music);
                octreeWorld.Update(gameTime, camera);
            }
            else if (levelHander.GetGameState == GameLevelHandler.GameState.END)
            {

            }
        }

        public void TriggerGameState(GameLevelHandler.GameState gameState)
        {

        }

        public override void Draw(GameTime gameTime)
        {
            if (levelHander.GetGameState == GameLevelHandler.GameState.PLAY)
            {
                octreeWorld.GetOctree().ModelsDrawn = 0;
                BoundingFrustum cameraFrustrum = new BoundingFrustum(camera.ViewMatrix * camera.ProjectionMatrix);
                octreeWorld.GetOctree().Draw(camera.ViewMatrix, camera.ProjectionMatrix, cameraFrustrum);
                //octreeWorld.GetOctree().DrawBoxLines(camera.ViewMatrix, camera.ProjectionMatrix, Game.GraphicsDevice, effect);                
                Game.Window.Title = string.Format("Models drawn: {0}", octreeWorld.GetOctree().ModelsDrawn);
                city.DrawCity(Game.GraphicsDevice, camera, floorEffect,0f, new Vector3(0, 0, 0));
                //city.DrawBoxLines(camera.ViewMatrix, camera.ProjectionMatrix, Game.GraphicsDevice, effect);
                camera.DrawWeapon();
                textHandler.DrawText(((Game1)Game).GetSpriteFont(), ((Game1)Game).GetSpriteBatch(), gameTime, Game.GraphicsDevice);
            }
            else if (levelHander.GetGameState == GameLevelHandler.GameState.END)
            {

            }
            


            base.Draw(gameTime);
        }

        
        public int GetPlayerScore()
        {
            return this.playerScore;
        }

        public void IncreasePlayerScore(int socre)
        {
            this.playerScore += socre;
        }

        public void DeductPlayerHealth(int health)
        {
            this.playerHealth -= health;
        }

        public GameLevelHandler.GameLevel GetGameLevel()
        {
            return levelHander.GetGameLevel;
        }

        public Music GetMusic()
        {
            return this.music;
        }

        public City GetCity()
        {
            return city;
        }

        public Octree GetOctreeWorld()
        {
            return this.octreeWorld;
        }

    }
}
