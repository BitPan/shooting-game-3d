using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using ShootingGame.Models;
using Microsoft.Xna.Framework.Graphics;
using ShootingGame.Data;
using GameData;
/*
 Class control the 
 */
namespace ShootingGame.Core
{
    public class Octree : Microsoft.Xna.Framework.GameComponent
    {
        SceneManager sceneManager;
        Random rnd;
        OcTreeNode octreeRoot;
        List<int> enemyIDs;
        List<int> playerIDs;
        List<int> enemyBulletIDs;
        List<int> playerBulletIDs;
        List<int> tankIDs;
        Player player;
        int timeLastStamp;
        TankModel tank;

        private int boundryLeft;
        private int boundryRight;
        private int boundryNear;
        private int boundryFar;
        private bool controlTankEnabled;

        private Model playerModel;
        private Model enemyPlaneModel;
        private Model playerBulletModel;
        private Model enemyBulletModel;
        private Model tankModel;
        private Model potionModel;

        public Octree(Game game, SceneManager sceneManager, GameWorldData worldData)
            : base(game)
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
            tankIDs = new List<int>();
            rnd = new Random();
            controlTankEnabled = false;
            timeLastStamp = 0;
        }

        public void Initialize(int[] enemyData, TankStatusMode tankstatus, GameTime gametime, PlayerData player)
        {
            AddPlayerModel(new Vector3(player.startPosition_x, 0, player.startPosition_z));
            AddTankModel(new Vector3(0, 0, 0), tankstatus);
        }

        public void Update(GameTime gameTime, FirstPersonCamera camera, int[] enemyData)
        {
            List<DrawableModel> models = new List<DrawableModel>();
            UpdateAndAddEnemy(gameTime, enemyData);
            octreeRoot.GetUpdatedModels(ref models);
            //Update All Models
            foreach (DrawableModel model in models)
            {
                if (model.GetType().ToString().Equals("ShootingGame.EnemyPlane"))
                {
                    EnemyPlane newModel = UpdateEnemyPlane(gameTime, (EnemyPlane)model, rnd);
                    octreeRoot.Add(newModel);

                }
                else if (model.GetType().ToString().Equals("ShootingGame.Player"))
                {
                    Player newPlayer = (Player)model;
                    newPlayer.DoTranslation(new Vector3((camera.Position - newPlayer.Position).X, 0, (camera.Position - newPlayer.Position).Z));
                    octreeRoot.Add(newPlayer);
                    player = newPlayer;

                }
                else if (model.GetType().ToString().Equals("ShootingGame.TankModel"))
                {
                    TankModel newTankModel = (TankModel)model;
                    newTankModel.Update(gameTime, player, rnd);
                    octreeRoot.Add(newTankModel);
                    this.tank = newTankModel;
                }
                else
                    octreeRoot.Add(model);
            }
            //Detect Collision
            List<int> modelsToRemove = new List<int>();
            List<int> bulletsToRemove = new List<int>();
            octreeRoot.DetectCollision(sceneManager.GetCity(), ref modelsToRemove, ref bulletsToRemove);

            foreach (int modelID in bulletsToRemove)
            {
                octreeRoot.RemoveDrawableModel(modelID);
            }

            foreach (int modelID in modelsToRemove)
            {
                DrawableModel model = octreeRoot.RemoveDrawableModel(modelID);
                if (model.GetType().ToString().Equals("ShootingGame.EnemyPlane"))
                {
                    sceneManager.IncreasePlayerScore(((EnemyPlane)model).GetEnemyScore());
                    sceneManager.GetMusic().EffectStopPlay();
                    sceneManager.GetMusic().EffectPlay();
                    sceneManager.GetExplosionHandler().CreateExplosion(model.Position);
                }
                else if (model.GetType().ToString().Equals("ShootingGame.EnemyBullet"))
                {
                    sceneManager.DeductPlayerHealth(10);
                    sceneManager.GetMusic().hitSoundPlay();
                }
                else if (model.GetType().ToString().Equals("ShootingGame.HealthGlobe"))
                    sceneManager.RecoverPlayerHealth(10);
            }
        }

        public void LoadModels(List<Model> models)
        {
            playerModel = models[1];
            enemyPlaneModel = models[1];
            playerBulletModel = models[2];
            enemyBulletModel = models[3];
            tankModel = models[4];
            potionModel = models[5];
        }

        public void UpdateAndAddEnemy(GameTime gametime, int[] enemyData)
        {
            timeLastStamp += gametime.ElapsedGameTime.Milliseconds;
                                   
            if (timeLastStamp >= enemyData[0])
            {
                float verticalPosition = (float)rnd.NextDouble() * 200 + 400;
                float horizontalPosition = (float)(rnd.NextDouble() * 1500 - 300);

                Vector3 position1 = new Vector3(horizontalPosition, verticalPosition, boundryFar - 1000);
                Vector3 direction1 = new Vector3(0, 0, 1);
                AddEnemyModel(position1, direction1, enemyData);
                timeLastStamp = 0;
            }
        }

        private void AddHealthGlobeModel(Vector3 position)
        {
            HealthGlobe newDModel = new HealthGlobe(potionModel, Matrix.CreateScale(0.3f) * Matrix.CreateTranslation(position.X, 8f, position.Z), Vector3.Zero);
            octreeRoot.Add(newDModel);
        }
        
        public void AddTankModel(Vector3 position, TankStatusMode tankStatus)
        {
            TankModel newDModel = new TankModel(tankStatus, tankModel, Matrix.CreateScale(0.08f) * Matrix.CreateTranslation(position.X, position.Y, position.Z), new Vector3(0, 1, 0), sceneManager.GetCity().GetCityMap());
            octreeRoot.Add(newDModel);
            tank = newDModel;
        }

        public void AddPlayerModel(Vector3 position)
        {
            Player newDModel = new Player(playerModel, Matrix.CreateScale(0.02f) * Matrix.CreateTranslation(position.X, position.Y + 20f, position.Z), Vector3.Zero);
            int id = octreeRoot.Add(newDModel);
            player = newDModel;
        }

        public void AddEnemyModel(Vector3 position, Vector3 direction, int[] enemyData)
        {
            DrawableModel newDModel = new EnemyPlane(enemyPlaneModel, Matrix.CreateTranslation(position.X, position.Y, position.Z), direction, enemyData);
            octreeRoot.Add(newDModel);
        }

        public void AddEnemyBulletModel(Vector3 position, Vector3 direction)
        {
            Vector3 bulletPosition = new Vector3(position.X + direction.X * 3, position.Y + direction.Y * 3, position.Z + direction.Z * 3);
            DrawableModel newDModel = new EnemyBullet(enemyBulletModel, Matrix.CreateTranslation(bulletPosition.X, bulletPosition.Y, bulletPosition.Z), direction);
            octreeRoot.Add(newDModel);
        }

        public void AddPlayerBulletModel(Vector3 position, Vector3 direction)
        {
            Vector3 bulletPosition = position + direction;
            DrawableModel newDModel = new Bullet(playerBulletModel, Matrix.CreateTranslation(position.X, position.Y, position.Z), direction * 20);
            octreeRoot.Add(newDModel);
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

        public void UseHealthGlobe()
        {
            if (tank.GetHealthGlobe() >= 1)
            {
                tank.DeductHealthGlobe();
                AddHealthGlobeModel(tank.Position);
            }
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

        public TankModel GetTank()
        {
            return this.tank;
        }

        public int GetTankHealthGlobe()
        {
            return tank.GetHealthGlobe();
        }

        public OcTreeNode GetOctree()
        {
            return octreeRoot;
        }

        public Player GetPlayer()
        {
            return player;
        }

        public bool IsControlTankEnabled()
        {
            return this.controlTankEnabled;
        }

        public void EnableControlTank()
        {
            this.controlTankEnabled = true;
        }

        public void DisableControlTank()
        {
            this.controlTankEnabled = false;
        }
    }
}
