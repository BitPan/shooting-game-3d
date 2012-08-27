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
    public class ModelManager : DrawableGameComponent
    {
        private Game game;
        List<SpinningModel> shots;
        List<SpinningModel> enemies;
        List<SpinningModel> enemyBullets;
        List<SpinningModel> player;

        Vector3 maxSpawnLocation = new Vector3(100, 100, -3000);
        private int timeSinceLastSpawn = 0;
        private int timeSinceEnemyLastShoot = 0;

        private const int FLYING_OUT_ZONE = 500;
        private const int PLAYER_BULLET_SPEED = 10;

        private static int enemySpawnCd = 0;
        private static int enemyShootCd = 0;
        private static int enemyAttackRange = 0;
        private static int enemyBulletSpeed = 0;
        private static int enemyMovingSpeed = 0;
        private static int enemyAttackDeviationFactor = 0;
        private static int deviationRange = 0;
        private static int enemyAttackChanceFactor = 0;

        private int[] levelData;

        private static int boundryLeft = -1000;
        private static int boundryRight = 1000;
        private static int boundryNear = 1500;
        private static int boundryFar = -1500;

        float maxRollAngle = MathHelper.Pi / 40;

        public int score { get; set; }
        public int playerHealth { get; set; }

        public ModelManager(Game game)
            : base(game)
        {
            this.game = game;
            // TODO: Construct any child components here
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here
            shots = new List<SpinningModel>();
            enemies = new List<SpinningModel>();
            enemyBullets = new List<SpinningModel>();
            player = new List<SpinningModel>(); 
            playerHealth = 100;
            score = 0;
            base.Initialize();
        }

        public void LoadGameLevelData(int[] levelData)
        {
            enemySpawnCd = levelData[0];
            enemyShootCd = levelData[1];
            enemyBulletSpeed = levelData[2];
            enemyMovingSpeed = levelData[3];
            enemyAttackDeviationFactor = levelData[4];
            enemyAttackChanceFactor = levelData[5];
            deviationRange = levelData[6];
            enemyAttackRange = levelData[7];            
        }


        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here

            detectCollision();
            updateEnemies(gameTime);
            updateEnemyBullets();
            updateShots();
            updatePlayer();
            
            base.Update(gameTime);
        }

        public void updateShots()
        {
            for (int i = 0; i < shots.Count(); i++)
            {
                shots[i].Update();
                if (shots[i].GetWorld().Translation.Z <= -1500 || shots[i].GetWorld().Translation.Z >= 1500)
                {
                    shots.RemoveAt(i);
                    i--;
                }
            }
        }

        public void updatePlayer()
        {
            for (int i = 0; i < player.Count(); i++)
            {
                player[i].Update();
            }
        }

        public void updateEnemyBullets()
        {
            for (int i = 0; i < enemyBullets.Count(); i++)
            {
                enemyBullets[i].Update();
                if (enemyBullets[i].GetWorld().Translation.Z >= 1500 || enemyBullets[i].GetWorld().Translation.Z <= -1500)
                {
                    enemyBullets.RemoveAt(i);
                    i--;
                }
            }
        }

        private void detectCollision()
        {
            for (int j = 0; j < shots.Count; j++)
            {
                for (int i = 0; i < enemies.Count; i++)
                {
                    if (shots[j].CollidesWith(enemies[i].model, enemies[i].GetWorld()))
                    {
                        shots.RemoveAt(j);
                        enemies.RemoveAt(i);
                        score += 10;
                        j--;
                    }
                }

                for (int h = 0; h < enemyBullets.Count; h++)
                {
                    if (shots[j].CollidesWith(enemyBullets[h].model, enemyBullets[h].GetWorld()))
                    {
                        shots.RemoveAt(j);
                        enemyBullets.RemoveAt(h);
                        j--;
                    }
                }
            }

            for (int j = 0; j < enemyBullets.Count; j++)
            {
                if (player[0].CollidesWith(enemyBullets[j].model, enemyBullets[j].GetWorld()))
                    {
                        enemyBullets.RemoveAt(j);
                        playerHealth -= 10;
                        j--;
                    }
            }
        }

        public void updateEnemies(GameTime gameTime)
        {
            for (int i = 0; i < enemies.Count(); i++)
            {                
                Vector3 enemyPosition = enemies[i].GetWorld().Translation;
                Vector3 playerPosition = player[0].GetWorld().Translation;
;

                enemies[i].Update();

                if (enemyPosition.Z - playerPosition.Z <= enemyAttackRange ||
                    enemyPosition.Z - playerPosition.Z >= -enemyAttackRange ||
                    enemyPosition.X - playerPosition.X <= enemyAttackRange ||
                    enemyPosition.X - playerPosition.X >= -enemyAttackRange)
                {
                    switch (((Game1)Game).rnd.Next(enemyAttackChanceFactor))
                    {
                        case 0:
                            Vector3 attackDirection = getAttackDirection(playerPosition, enemyPosition);
                            enemyAttack(gameTime, enemyPosition, attackDirection);
                            break;                        
                    }
                }

                if (isFlyingOutOfBoundry(enemies[i]))
                {
                    float enemyCurrentZSpeed = enemies[i].GetDirection().Z;
                    float speedToReduce = (enemies[i].GetOriginalSpeed())/10;

                    float enemyZPositionAfterMoving = (enemyPosition.Z + enemyCurrentZSpeed - speedToReduce);

                    float speedX = (enemies[i].GetDirection().X < 4) ? (enemies[i].GetOriginalSpeed() - 0) / 10 : -(enemies[i].GetOriginalSpeed()) / 10;
                    float enemySpeedAfterChange = enemies[i].GetDirection().X + speedX;

                    float enemyXPositionAfterMoving = enemyPosition.X + enemySpeedAfterChange;
                    float xDifference = (float)(enemyXPositionAfterMoving - enemyPosition.X);
                    float zDifference = (float)(enemyZPositionAfterMoving - enemyPosition.Z);
                    
                    Vector3 flyDirection = new Vector3(xDifference, 0, zDifference);
                    enemies[i].setDirection(flyDirection);
                }

                if (enemies[i].GetWorld().Translation.Z > 1500 || enemies[i].GetWorld().Translation.Z < - 1500)
                {
                    enemies.RemoveAt(i);
                    i--;
                }   
            }
        }

        private Boolean isFlyingOutOfBoundry(BasicModel model)
        {
            Vector3 modelPosition = model.GetWorld().Translation;
            float modelXDirection = model.direction.X;
            float modelZDirection = model.direction.Z;

            float boundryInX = (modelXDirection > 0) ? boundryRight : boundryLeft;
            float boundryInZ = (modelZDirection > 0) ? boundryNear : boundryFar;

            if ((modelPosition.X - boundryInX) <= FLYING_OUT_ZONE && (modelPosition.X - boundryInX) >= -FLYING_OUT_ZONE)
                return true;
            if ((modelPosition.Z - boundryInZ) <= FLYING_OUT_ZONE && (modelPosition.Z - boundryInZ) >= -FLYING_OUT_ZONE)
                return true;
            return false;
        }

        public float getAttackDevaition(int rndNo)
        {
            switch (rndNo)
            {
                case 0:
                    return (float)((Game1)Game).rnd.NextDouble() * deviationRange;
                case 1:
                    return (float)((Game1)Game).rnd.NextDouble() * deviationRange / 2;
                case 2:
                    return (float)((Game1)Game).rnd.NextDouble() * deviationRange / 4;
                default:
                    return 0;
            }
        }

        private Vector3 getAttackDirection(Vector3 playerPosition, Vector3 enemyPosition)
        {
            float attackXDeviation = getAttackDevaition(((Game1)Game).rnd.Next(enemyAttackDeviationFactor));
            float attackYDeviation = getAttackDevaition(((Game1)Game).rnd.Next(enemyAttackDeviationFactor));
            float attackZDeviation = getAttackDevaition(((Game1)Game).rnd.Next(enemyAttackDeviationFactor));
            float xDifference = (float)(playerPosition.X - enemyPosition.X) + attackXDeviation;
            float yDifference = (float)(playerPosition.Y - enemyPosition.Y) + attackYDeviation;
            float zDifference = (float)(playerPosition.Z - enemyPosition.Z) + attackZDeviation;

            float distance = (float)Math.Sqrt((double)(xDifference * xDifference + yDifference * yDifference + zDifference * zDifference));
            float time = (distance / enemyBulletSpeed);
            Vector3 attackDirection = new Vector3(xDifference / time, yDifference / time, zDifference / time);
            return attackDirection;
        }

        private void enemyAttack(GameTime gameTime, Vector3 position, Vector3 direction)
        {
            timeSinceEnemyLastShoot += gameTime.ElapsedGameTime.Milliseconds;
            if (timeSinceEnemyLastShoot >= enemyShootCd)
            {
                enemyBullets.Add(new SpinningModel(Game.Content.Load<Model>("Models\\ammo"), position, direction, 0, 0, 0));
                timeSinceEnemyLastShoot = 0;
            }
        }

        public void spawnEnemy(GameTime gameTime)
        {
            timeSinceLastSpawn += gameTime.ElapsedGameTime.Milliseconds;
            if (timeSinceLastSpawn >= enemySpawnCd)
            {
                float verticalPosition = (float)((Game1)Game).rnd.NextDouble() * 100 + 100;
                float horizontalPosition = (float)((Game1)Game).rnd.NextDouble() * 500 - 250;
                Vector3 position = new Vector3(horizontalPosition, verticalPosition, -1500);
                Vector3 direction = new Vector3(0, 0, enemyMovingSpeed);
                float rollRotation = (float)((Game1)Game).rnd.NextDouble() * maxRollAngle - (maxRollAngle / 2);
                enemies.Add(new SpinningModel(Game.Content.Load<Model>("Models\\spaceship"), position, direction, 0, 0, rollRotation));
                timeSinceLastSpawn = 0;
            }
        }
                
        public void addShot(Vector3 position, Vector3 direction)
        {
            shots.Add(new SpinningModel(Game.Content.Load<Model>("Models\\ammo"), position, direction * PLAYER_BULLET_SPEED, 0, 0, 0));
        }

        public void addPlayer(Vector3 position, Vector3 direction)
        {
            player.Add(new SpinningModel(Game.Content.Load<Model>("Models\\spaceship"), position, direction, 0, 0, 0));
        }

        public override void Draw(GameTime gameTime)
        {

            foreach (BasicModel enemy in enemies)
            {
                enemy.Draw(((Game1)Game).camera);
            }
            foreach (BasicModel eb in enemyBullets)
            {
                eb.Draw(((Game1)Game).camera);
            }
            foreach (BasicModel p in player)
            {
                p.Draw(((Game1)Game).camera);
            }
            foreach (BasicModel shot in shots)
            {
                shot.Draw(((Game1)Game).camera);
            }
            base.Draw(gameTime);
        }
    }

        
    
}
