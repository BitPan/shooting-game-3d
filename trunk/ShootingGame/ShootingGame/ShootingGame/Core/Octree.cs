using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using ShootingGame.Models;
using Microsoft.Xna.Framework.Graphics;
using ShootingGame.Data;

namespace ShootingGame.Core
{
    public class Octree
    {
        SceneManager sceneManager;
        Random rnd;
        OcTreeNode octreeRoot;
        List<int> enemyIDs;
        List<int> playerIDs;
        List<int> enemyBulletIDs;
        List<int> playerBulletIDs;
        Player player;
        private int boundryLeft;
        private int boundryRight;
        private int boundryNear;
        private int boundryFar;
        

        private Model playerModel;
        private Model enemyPlaneModel;
        private Model playerBulletModel;
        private Model enemyBulletModel;


        public Octree(SceneManager sceneManager, GameWorldData worldData)
        {
            this.sceneManager = sceneManager;
            octreeRoot = new OcTreeNode(worldData.OCTREE_WORLD_CENTER1, worldData.OCTREE_WORLD_SIZE1);
            octreeRoot.RootSize = worldData.OCTREE_WORLD_SIZE1;
            boundryLeft = worldData.BoundryLeft;
            boundryRight = worldData.BoundryRight;
            boundryNear = worldData.BoundryNear;
            boundryFar = worldData.BoundryFar;
            enemyIDs = new List<int>();
            playerIDs = new List<int>();
            enemyBulletIDs = new List<int>();
            playerBulletIDs = new List<int>();
            rnd = new Random();
        }

        public void TestInitialize(int[] enemyData)
        {
            AddPlayerModel(new Vector3(500, 0, -500));
            float verticalPosition = (float)rnd.NextDouble() * 100 + 500;
            float horizontalPosition = (float)rnd.NextDouble() * 1000 - 100;
            

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
        }

        public void Update(GameTime gameTime, FirstPersonCamera camera)
        {
            List<DrawableModel> models = new List<DrawableModel>();
            octreeRoot.GetUpdatedModels(ref models);

            //Update All Models
            foreach (DrawableModel model in models)
            {
                if (model.GetType().ToString().Equals("ShootingGame.EnemyPlane"))
                {
                    EnemyPlane newModel = UpdateEnemyPlane(gameTime, (EnemyPlane)model, rnd);
                    octreeRoot.Add(newModel);
                }
                else if(model.GetType().ToString().Equals("ShootingGame.Player"))
                {
                    Player newPlayer = (Player)model;
                    newPlayer.DoTranslation(new Vector3((camera.Position - newPlayer.Position).X, 0, (camera.Position - newPlayer.Position).Z));
                    octreeRoot.Add(newPlayer);
                    player = newPlayer;
                }
                else
                octreeRoot.Add(model);
            }

            //Detect Collision
            List<int> modelsToRemove = new List<int>();
            octreeRoot.DetectCollision(ref modelsToRemove);

            foreach (int modelID in modelsToRemove)
            {                
                DrawableModel model = octreeRoot.RemoveDrawableModel(modelID);
                if (model.GetType().ToString().Equals("ShootingGame.EnemyPlane"))
                {
                    sceneManager.IncreasePlayerScore(10);
                    sceneManager.GetMusic().EffectStopPlay();
                    sceneManager.GetMusic().EffectPlay();
                }
                if (model.GetType().ToString().Equals("ShootingGame.EnemyBullet"))
                {
                    sceneManager.DeductPlayerHealth(10);
                    sceneManager.GetMusic().hitSoundPlay();                    
                }
            }
        }

        
        public void LoadModels(List<Model> models)
        {
            playerModel = models[1];
            enemyPlaneModel = models[1];
            playerBulletModel = models[2];
            enemyBulletModel = models[3];
        }

        public void AddPlayerModel(Vector3 position)
        {
            Player newDModel = new Player(playerModel, Matrix.CreateScale(0.01f) * Matrix.CreateTranslation(position.X, position.Y + 20f, position.Z), Vector3.Zero);
            int id = octreeRoot.Add(newDModel);
            playerIDs.Add(id);
            player = newDModel;
        }

        public void AddEnemyModel(Vector3 position, Vector3 direction, int[] enemyData)
        {
            DrawableModel newDModel = new EnemyPlane(enemyPlaneModel, Matrix.CreateTranslation(position.X, position.Y, position.Z), direction, enemyData);
            int id = octreeRoot.Add(newDModel);
            enemyIDs.Add(id);
        }

        public void AddEnemyBulletModel(Vector3 position, Vector3 direction)
        {
            Vector3 bulletPosition = new Vector3(position.X + direction.X * 3, position.Y + direction.Y * 3, position.Z + direction.Z * 3);
            DrawableModel newDModel = new EnemyBullet(enemyBulletModel, Matrix.CreateTranslation(bulletPosition.X, bulletPosition.Y, bulletPosition.Z), direction);
            int id = octreeRoot.Add(newDModel);

            enemyIDs.Add(id);
        }

        public void AddPlayerBulletModel(Vector3 position, Vector3 direction)
        {
            Vector3 bulletPosition = position + direction;
            DrawableModel newDModel = new Bullet(playerBulletModel, Matrix.CreateTranslation(position.X, position.Y, position.Z), direction * 20);
            int id = octreeRoot.Add(newDModel);
            playerBulletIDs.Add(id);
        }

        private EnemyPlane UpdateEnemyPlane(GameTime gameTime, EnemyPlane enemy, Random rnd)
        {
            EnemyPlane newPlane = enemy;
            if (newPlane.canEnemyAttack(gameTime, rnd, player.Position))
                AddEnemyBulletModel(enemy.Position, enemy.getAttackDirection(rnd, player.Position));

            if (isFlyingOutOfBoundry(newPlane))
                newPlane.CalculateEnemyTurnAround(rnd);

            if (newPlane.IsTurningAround() == true)
                newPlane.PerformTurnAction(player.Position);
            return newPlane;
        }

        private bool isFlyingOutOfBoundry(DrawableModel model)
        {
            int FLYING_OUT_ZONE = 500;
            Vector3 modelPosition = model.Position;
            float modelXDirection = model.Direction.X;
            float modelZDirection = model.Direction.Z;

            float boundryInX = (modelXDirection > 0) ? boundryRight : boundryLeft;
            float boundryInZ = (modelZDirection > 0) ? boundryNear : boundryFar;

            if (Math.Abs(modelPosition.X - boundryInX) <= FLYING_OUT_ZONE)
                return true;
            if (Math.Abs(modelPosition.Z - boundryInZ) <= FLYING_OUT_ZONE)
                return true;
            return false;
        }

        public OcTreeNode GetOctree()
        {
            return octreeRoot;
        }

        public Player GetPlayer()
        {
            return player;
        }
    }
}
