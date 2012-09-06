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
        List<SpinningModel> shots, enemies, enemyBullets, player, trees;

        private int timeSinceLastSpawn,timeSinceEnemyLastShoot;

        private const int FLYING_OUT_ZONE = 500;
        private const int PLAYER_BULLET_SPEED = 20;
        private const float SPACE_SHIP_SACLE_FACTOR = 20;
        float maxRollAngle = MathHelper.Pi / 40;

        private static int enemySpawnCd = 0;
        private static int enemyShootCd = 0;
        private static int enemyAttackRange = 0;
        private static int enemyBulletSpeed = 0;
        private static int enemyMovingSpeed = 0;
        private static int enemyAttackDeviationFactor = 0;
        private static int enemyTurnAroundFactor = 0;
        private static int deviationRange = 0;
        private static int enemyAttackChanceFactor = 0;

        private const int boundryLeft = -1000;
        private const int boundryRight = 1000;
        private const int boundryNear = 2000;
        private const int boundryFar = -2000;
        
        public int score { get; set; }

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
            trees = new List<SpinningModel>();
            timeSinceLastSpawn = 0;
            timeSinceEnemyLastShoot = 0;
            score = 0;
            base.Initialize();         
        }

        public Model LModel(Effect effect, string assetName, out Texture2D[] textures)
        {

            Model newModel = this.Game.Content.Load<Model>(@assetName);
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
            enemyTurnAroundFactor = levelData[8];
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
                if (shots[i].GetWorld().Translation.Z >= boundryNear ||
                    shots[i].GetWorld().Translation.Z <= boundryFar ||
                    shots[i].GetWorld().Translation.Y < 0 ||
                    shots[i].GetWorld().Translation.X < boundryLeft ||
                    shots[i].GetWorld().Translation.X > boundryRight)
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
                if (enemyBullets[i].GetWorld().Translation.Z >= boundryNear ||
                    enemyBullets[i].GetWorld().Translation.Z <= boundryFar || 
                    enemyBullets[i].GetWorld().Translation.Y < 0 ||
                    enemyBullets[i].GetWorld().Translation.X < boundryLeft ||
                    enemyBullets[i].GetWorld().Translation.X > boundryRight)
                {
                    enemyBullets.RemoveAt(i);
                    i--;
                }
            }
        }

        private void detectCollision()
        {
            for (int j = 0; j < enemies.Count; j++)
            {
                for (int i = 0; i < shots.Count; i++)
                {
                    if (shots[i].CollidesWith(enemies[j].model, enemies[j].GetWorld()))
                    {
                        enemies.RemoveAt(j);
                        shots.RemoveAt(i);                        
                        score += 10;
                        j--;
                    }
                }                
            }

            for (int j = 0; j < shots.Count; j++)
            {
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
                        ((Game1)Game).DeductPlayerHealth(10);
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
                    switch (((Game1)Game).rnd.Next(enemyTurnAroundFactor))
                    {
                        case 0:
                            if (enemies[i].TurnAround() == false)
                                enemies[i].ChangeDoTurnAround();
                            break;
                    }

                }

                if (enemies[i].TurnAround() == true)
                {
                    float enemyCurrentXSpeed = enemies[i].GetDirection().X;
                    float enemyCurrentZSpeed = enemies[i].GetDirection().Z;
                    float speedToReduce = (enemies[i].GetOriginalSpeed()) / 20;
                    
                    float speedToReduceInX = (enemies[i].GetOriginalPosition().X - player[0].GetWorld().Translation.X>0) ? -speedToReduce : speedToReduce;
                    
                    float speedX = 0;

                    if (speedToReduceInX >= 0)
                    {
                        if (enemies[i].AtMiddle() == false)
                        {
                            if (enemyCurrentXSpeed >= 4)
                            {
                                speedX = -speedToReduceInX;
                                enemies[i].ChangeAtMiddle(true);
                            }
                            else
                                speedX = speedToReduceInX;
                        }
                        else
                        {
                            if (enemyCurrentXSpeed <= 0)
                            {
                                speedX = 0;
                                enemies[i].ChangeAtMiddle(false);
                            }
                            else
                                speedX = -speedToReduceInX;
                        }
                    }
                    else if (speedToReduceInX < 0)
                    {
                        if (enemies[i].AtMiddle() == false)
                        {
                            if (enemyCurrentXSpeed <= -4)
                            {
                                speedX = -speedToReduceInX;
                                enemies[i].ChangeAtMiddle(true);
                            }
                            else
                                speedX = speedToReduceInX;
                        }
                        else
                        {
                            if (enemyCurrentXSpeed >= 0)
                            {
                                speedX = 0;
                                enemies[i].ChangeAtMiddle(false);
                            }
                            else
                                speedX = -speedToReduceInX;
                        }
                    }

                    if (enemyCurrentZSpeed < -enemies[i].GetOriginalSpeed())
                    {
                        enemyCurrentZSpeed = -enemies[i].GetOriginalSpeed();
                        speedToReduce = 0;
                        speedX = 0;
                        enemies[i].ChangeDoTurnAround();
                        enemies[i].yawRotate((float)Math.PI);
                        enemies[i].SetOriginalPosition(enemies[i].GetWorld().Translation);
                    }
                    //float speedX = (enemies[i].GetDirection().X < 4) ? (enemies[i].GetOriginalSpeed() - 0) / 10 : -(enemies[i].GetOriginalSpeed()) / 10;
                    float enemySpeedAfterChange = enemyCurrentXSpeed + speedX;
                    float enemyZPositionAfterMoving = (enemyPosition.Z + enemyCurrentZSpeed - speedToReduce);

                    float enemyXPositionAfterMoving = enemyPosition.X + enemySpeedAfterChange;
                    float xDifference = (float)(enemyXPositionAfterMoving - enemyPosition.X);
                    float zDifference = (float)(enemyZPositionAfterMoving - enemyPosition.Z);

                    Vector3 flyDirection = new Vector3(xDifference, 0, zDifference);
                    enemies[i].setDirection(flyDirection);
                }

                if (enemyPosition.Z >= boundryNear ||
                    enemyPosition.Z <= boundryFar ||
                    enemyPosition.Y < 0 ||
                    enemyPosition.X < boundryLeft ||
                    enemyPosition.X > boundryRight)
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
                enemyBullets.Add(new SpinningModel(Game.Content.Load<Model>("Models\\ammo"), 1, position, direction, 0, 0, 0));
                timeSinceEnemyLastShoot = 0;
            }
        }

        public void spawnEnemy(GameTime gameTime)
        {
            timeSinceLastSpawn += gameTime.ElapsedGameTime.Milliseconds;
            if (timeSinceLastSpawn >= enemySpawnCd)
            {
                float verticalPosition = (float)((Game1)Game).rnd.NextDouble() * 100 + 100;
                float horizontalPosition = (float)((Game1)Game).rnd.NextDouble() * 1000 - 250;
                Vector3 position = new Vector3(horizontalPosition, verticalPosition, boundryFar+500);
                Vector3 direction = new Vector3(0, 0, enemyMovingSpeed);
                float rollRotation = (float)((Game1)Game).rnd.NextDouble() * maxRollAngle - (maxRollAngle / 2);
                enemies.Add(new SpinningModel(Game.Content.Load<Model>("junctioned"), SPACE_SHIP_SACLE_FACTOR, position, direction, 0, 0, 0));
                
                //enemies[enemies.Count() - 1].setWorld(Matrix.CreateScale(10f));
                timeSinceLastSpawn = 0;
            }
        }
                
        public void addShot(Vector3 position, Vector3 direction)
        {
            shots.Add(new SpinningModel(Game.Content.Load<Model>("Models\\ammo"), 1, position, direction * PLAYER_BULLET_SPEED, 0, 0, 0));
        }

        public void addPlayer(Vector3 position, Vector3 direction)
        {
            player.Add(new SpinningModel(Game.Content.Load<Model>("Models\\spaceship"), 0.3f, position, direction, 0, 0, 0));
        }

        public void addTree(Vector3 position)
        {
            trees.Add(new SpinningModel(Game.Content.Load<Model>("Models\\spaceship"), 1f, position, Vector3.Zero, 0, 0, 0));
        }

        public SpinningModel GetPlayer()
        {
            return player[0];
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
            foreach (BasicModel t in trees)
            {
                t.Draw(((Game1)Game).camera);
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

        public void DrawGround(GraphicsDevice device, Camera camera, Model ground, Texture2D[] groundTextures)
        {
            SamplerState ss = new SamplerState();
            ss.AddressU = TextureAddressMode.Clamp;
            ss.AddressV = TextureAddressMode.Clamp;
            device.SamplerStates[0] = ss;

            DepthStencilState dss = new DepthStencilState();
            dss.DepthBufferEnable = false;
            device.DepthStencilState = dss;

            Matrix[] groundTransforms = new Matrix[ground.Bones.Count];
            ground.CopyAbsoluteBoneTransformsTo(groundTransforms);
            int i = 0;
            foreach (ModelMesh mesh in ground.Meshes)
            {
                foreach (Effect currentEffect in mesh.Effects)
                {
                    Matrix worldMatrix = groundTransforms[mesh.ParentBone.Index] * Matrix.CreateTranslation(new Vector3(0, 0, 0));
                    currentEffect.CurrentTechnique = currentEffect.Techniques["Textured"];
                    currentEffect.Parameters["xWorld"].SetValue(worldMatrix);
                    currentEffect.Parameters["xView"].SetValue(camera.view);
                    currentEffect.Parameters["xProjection"].SetValue(camera.projection);
                    currentEffect.Parameters["xTexture"].SetValue(groundTextures[i++]);
                }
                mesh.Draw();
            }

            dss = new DepthStencilState();
            dss.DepthBufferEnable = true;
            device.DepthStencilState = dss;
        }


 
     

        public void DrawSkybox(GraphicsDevice device, Camera camera, Model skyboxModel, Texture2D[] skyboxTextures)
        {
            SamplerState ss = new SamplerState();
            ss.AddressU = TextureAddressMode.Clamp;
            ss.AddressV = TextureAddressMode.Clamp;
            device.SamplerStates[0] = ss;

            DepthStencilState dss = new DepthStencilState();
            dss.DepthBufferEnable = false;
            device.DepthStencilState = dss;
            Matrix[] skyboxTransforms = new Matrix[skyboxModel.Bones.Count];
            skyboxModel.CopyAbsoluteBoneTransformsTo(skyboxTransforms);
            int i = 0;
            foreach (ModelMesh mesh in skyboxModel.Meshes)
            {
                foreach (Effect currentEffect in mesh.Effects)
                {
                    Matrix worldMatrix = Matrix.CreateScale(200) * skyboxTransforms[mesh.ParentBone.Index] * Matrix.CreateTranslation(camera.cameraPostion - (new Vector3(0, 50, 0)));
                    currentEffect.CurrentTechnique = currentEffect.Techniques["Textured"];
                    currentEffect.Parameters["xWorld"].SetValue(worldMatrix);
                    currentEffect.Parameters["xView"].SetValue(camera.view);
                    currentEffect.Parameters["xProjection"].SetValue(camera.projection);
                    currentEffect.Parameters["xTexture"].SetValue(skyboxTextures[i++]);
                }
                mesh.Draw();
            }

            dss = new DepthStencilState();
            dss.DepthBufferEnable = true;
            device.DepthStencilState = dss;
        }
        
    }

        
    
}
