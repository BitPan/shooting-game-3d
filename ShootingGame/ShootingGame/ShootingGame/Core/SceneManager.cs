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
        List<int> enemyIDs;
        List<int> playerIDs;
        List<int> enemyBulletIDs;
        List<int> playerBulletIDs;
        LevelData levelData;
        DrawableModel player;
        Model playerModel;
        Model enemyModel;
        Model bulletModel;
        Random rnd;
        int[] enemyData;

        private const int boundryLeft = -1000;
        private const int boundryRight = 1000;
        private const int boundryNear = 2000;
        private const int boundryFar = -2000;
        private const int FLYING_OUT_ZONE = 500;
        private const int PLAYER_BULLET_SPEED = 20;

 

        public OcTreeNode GetOcTreeRoot { get { return ocTreeRoot; } }

        public SceneManager(Game game)
            : base(game)
        {
            this.game = game;
            enemyIDs = new List<int>();
            playerIDs = new List<int>();
            enemyBulletIDs = new List<int>();
            playerBulletIDs = new List<int>();
            rnd = new Random();
            levelData = new LevelData();
            enemyData = levelData.loadLevelData(Game1.GameLevel.LEVEL1);
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            float verticalPosition = (float)rnd.NextDouble() * 100 + 100;
            float horizontalPosition = (float)rnd.NextDouble() * 1000 - 250;
            ocTreeRoot = new OcTreeNode(new Vector3(500, 0, -300), 2000);
            ocTreeRoot.RootSize = 2000;
            AddPlayerModel(new Vector3(500, 0, -500));

            Vector3 position1 = new Vector3(horizontalPosition, verticalPosition, boundryFar + 500);
            Vector3 direction1 = new Vector3(0, 0, 4);


            for (int i = 0; i < 20; i++)
            {
                verticalPosition = (float)rnd.NextDouble() * 100 + 100;
                horizontalPosition = (float)rnd.NextDouble() * 1000 - 250;
                position1 = new Vector3(horizontalPosition, verticalPosition, boundryFar + 500);
                direction1 = new Vector3(0, 0, 1);
                AddEnemyModel(position1, direction1, enemyData);
            }
                
            
            base.Initialize();
        }

        public void AddPlayerModel(Vector3 position)
        {
            DrawableModel newDModel = new DrawableModel(Game.Content.Load<Model>("Models\\player"), Matrix.CreateTranslation(position.X, position.Y, position.Z), Vector3.Zero);
            int id = ocTreeRoot.Add(newDModel);
            playerIDs.Add(id);
            player = newDModel;
        }

        public void AddEnemyModel(Vector3 position, Vector3 direction, int[] enemyData)
        {
            DrawableModel newDModel = new EnemyPlane(Game.Content.Load<Model>("Models\\ship"), Matrix.CreateTranslation(position.X, position.Y, position.Z), direction, enemyData);
            int id = ocTreeRoot.Add(newDModel);
            enemyIDs.Add(id);
        }

        public void AddEnemyBulletModel(Vector3 position, Vector3 direction)
        {
            Vector3 bulletPosition = new Vector3(position.X + direction.X * 3, position.Y + direction.Y * 3, position.Z + direction.Z * 3);
            DrawableModel newDModel = new Bullet(Game.Content.Load<Model>("Models\\ammo"), Matrix.CreateTranslation(bulletPosition.X, bulletPosition.Y, bulletPosition.Z), direction*20);
            int id = ocTreeRoot.Add(newDModel);
            
            enemyIDs.Add(id);
        }

        public void AddPlayerBulletModel(Vector3 position, Vector3 direction)
        {
            Vector3 bulletPosition = position + direction;            
            DrawableModel newDModel = new Bullet(Game.Content.Load<Model>("Models\\ammo"), Matrix.CreateTranslation(position.X, position.Y, position.Z), direction*20);
            int id = ocTreeRoot.Add(newDModel);
            playerBulletIDs.Add(id);
        }

        public void UpdatePlayerModel(Model myModel, Vector3 position)
        {
            for (int i = 0; i < playerIDs.Count; i++ )
            {
                ocTreeRoot.UpdateModelWorldMatrix(playerIDs[i]);
            }
        }

        public void UpdateEnemyModel(GameTime gameTime)
        {
            for (int i = 0; i < enemyIDs.Count; i++)
            {
                EnemyPlane enemy = (EnemyPlane)ocTreeRoot.UpdateModelWorldMatrix(enemyIDs[i]);
                if(enemy.canEnemyAttack(gameTime, rnd, player.Position))
                    AddEnemyBulletModel(enemy.Position, enemy.getAttackDirection(rnd, player.Position ));

                if (isFlyingOutOfBoundry(enemy) && enemy.IsTurningAround() == false)
                    enemy.CalculateEnemyTurnAround(rnd);

                if (enemy.IsTurningAround() == true)
                    enemy.PerformTurnAction(player.Position);

                if (isOutOfBoundry(enemy))
                {
                    ocTreeRoot.RemoveDrawableModel(enemyIDs[i]);
                    enemyIDs.Remove(enemyIDs[i]);
                }
            }            
        }

        private bool isFlyingOutOfBoundry(DrawableModel model)
        {
            Vector3 modelPosition = model.Position;
            float modelXDirection = model.Direction.X;
            float modelZDirection = model.Direction.Z;

            float boundryInX = (modelXDirection > 0) ? boundryRight : boundryLeft;
            float boundryInZ = (modelZDirection > 0) ? boundryNear : boundryFar;

            if ((modelPosition.X - boundryInX) <= FLYING_OUT_ZONE && (modelPosition.X - boundryInX) >= -FLYING_OUT_ZONE)
                return true;
            if ((modelPosition.Z - boundryInZ) <= FLYING_OUT_ZONE && (modelPosition.Z - boundryInZ) >= -FLYING_OUT_ZONE)
                return true;
            return false;
        }

        private Boolean isOutOfBoundry(DrawableModel model)
        {
            return model.Position.Z >= boundryNear ||
                    model.Position.Z <= boundryFar ||
                    model.Position.Y < 0 ||
                    model.Position.X < boundryLeft ||
                    model.Position.X > boundryRight;
        }

        public void UpdateEnemyBulletModel(Model myModel, Vector3 position)
        {
            for (int i = 0; i < enemyBulletIDs.Count; i++)
            {
                ocTreeRoot.UpdateModelWorldMatrix(enemyBulletIDs[i]);
            }
        }

        public void UpdatePlayerBulletModel()
        {
            for (int i = 0; i < playerBulletIDs.Count; i++)
            {
                ocTreeRoot.UpdateModelWorldMatrix(playerBulletIDs[i]);
            }
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

            //ocTreeRoot.DetectCollision();
            List<DrawableModel> models = new List<DrawableModel>();
            ocTreeRoot.GetUpdatedModels(ref models);

            foreach (DrawableModel model in models)
                ocTreeRoot.Add(model);

            List<int> modelsToRemove = new List<int>();
            ocTreeRoot.DetectCollision(ref modelsToRemove);

            foreach (int modelID in modelsToRemove)
                ocTreeRoot.RemoveDrawableModel(modelID);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
    }
}
