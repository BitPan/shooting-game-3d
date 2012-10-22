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


namespace ShootingGame.Core
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class SceneManager : DrawableGameComponent
    {
        public OcTreeNode ocTreeRoot;

        Game game;
        City city;
        LevelData levelData;
        Random rnd;
        int[] enemyData;
        Octree octreeWorld;
        private int playerHealth;
        GameLevel currentGameLevel;

        public enum GameLevel { LEVEL1, LEVEL2, LEVEL3, LEVEL4, LEVEL5 };

        private const int boundryLeft = -1000;
        private const int boundryRight = 1000;
        private const int boundryNear = 2000;
        private const int boundryFar = -2000;
        private const int FLYING_OUT_ZONE = 500;
        private const int PLAYER_BULLET_SPEED = 20;


        public int GetPlayerHealth { get { return playerHealth; } }
        public OcTreeNode GetOcTreeRoot { get { return octreeWorld.GetOctree(); } }
        public GameLevel GetGameLevel { get { return currentGameLevel; } }

        public SceneManager(Game game)
            : base(game)
        {
            this.game = game;
            city = new City();
            rnd = new Random();
            LoadWorldData();
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            LoadWorldModels();
            octreeWorld.TestInitialize(rnd, enemyData);
            currentGameLevel = GameLevel.LEVEL1;
            playerHealth = 100;
            base.Initialize();
        }


        public void AddPlayerBulletModel(Vector3 position, Vector3 direction)
        {
            octreeWorld.AddPlayerBulletModel(position, direction);
        }

        private void LoadWorldData()
        {
            int[] worldData = { boundryLeft, boundryRight, boundryNear, boundryFar };
            octreeWorld = new Octree(new Vector3(500, 0, -300), 2000, worldData);
            levelData = new LevelData();
            enemyData = levelData.loadLevelData(currentGameLevel);
        }

        private void LoadWorldModels()
        {
            List<Model> models = new List<Model>();
            models.Add(Game.Content.Load<Model>("Models\\player"));
            models.Add(Game.Content.Load<Model>("Models\\ship"));
            models.Add(Game.Content.Load<Model>("Models\\ammo"));
            models.Add(Game.Content.Load<Model>("Models\\ammo"));
            octreeWorld.LoadModels(models);
        }

        public City GetCity()
        {
            return city;
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

            octreeWorld.Update(gameTime, rnd, ((Game1)Game).GetGameCamera());
            //octreeWorld.UpdatePlayer(((Game1)Game).GetGameCamera().Position);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
    }
}
