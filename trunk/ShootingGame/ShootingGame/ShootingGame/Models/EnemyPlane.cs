using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ShootingGame.Models;

namespace ShootingGame
{
    public class EnemyPlane : DrawableModel
    {
        private int enemySpawnCd = 0;
        private int enemyShootCd = 0;
        private int enemyAttackRange = 0;
        private int enemyBulletSpeed = 0;
        private int enemyMovingSpeed = 0;
        private int enemyAttackDeviationFactor = 0;
        private int enemyTurnAroundFactor = 0;
        private int deviationRange = 0;
        private int enemyAttackChanceFactor = 0;
        private int health;
        private int enemyScore;
        private float scale;

        float originalSpeed = 0;
        Vector3 originalPosition;
        bool doTurnAround;
        bool turnToMiddle;
        private int timeSinceLastShoot;

        public EnemyPlane(Model inModel, Matrix inWorldMatrix, Vector3 newDirection, int[] enemyData)
            : base(inModel, inWorldMatrix, newDirection)
        {
            worldMatrix = Matrix.CreateScale(0.03f) * inWorldMatrix;     
            direction = newDirection * 4;
            originalPosition = Position;
            originalSpeed = direction.Z;
            timeSinceLastShoot = 0;
            loadEnemyData(enemyData);
            doTurnAround = false;
            health = enemyData[9];
            enemyScore = enemyData[10];
            this.rotation = Matrix.CreateFromYawPitchRoll((float)Math.PI,0,0);
            //yawRotate((float)Math.PI/2);
        }

        public int GetEnemyScore()
        {
            return this.enemyScore;
        }

        public void DoTranslation(Vector3 translation)
        {
            worldMatrix *= Matrix.CreateTranslation(translation);
        }

        public void setDirection(Vector3 direction)
        {
            this.direction = direction;
        }

        public override void Update()
        {
            //yawRotate((float)Math.PI / 180f);
            WorldMatrix = worldMatrix * Matrix.CreateTranslation(direction);
            
        }

        public void yawRotate(float rawRotate)
        {
            rotation *= Matrix.CreateFromYawPitchRoll(rawRotate, 0, 0);
        }

        public bool IsTurningAround()
        {
            return doTurnAround;
        }

        public bool AtMiddle()
        {
            return turnToMiddle;
        }

        public void ChangeDoTurnAround()
        {
            doTurnAround = (doTurnAround == true) ? false : true;
        }

        public void ChangeAtMiddle(bool bo)
        {
            turnToMiddle = bo;
        }

        public Vector3 GetDirection()
        {
            return direction;
        }

        public Vector3 GetOriginalPosition()
        {
            return originalPosition;
        }


        public float GetOriginalSpeed()
        {
            return originalSpeed;
        }

        public void SetOriginalPosition(Vector3 position)
        {
            originalPosition = position;
        }

        public void loadEnemyData(int[] levelData)
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

        public void PerformTurnAction(Vector3 playerPosition)
        {
            float speedToReduce = (originalSpeed) / 20;

            float speedToReduceInX = (originalPosition.X - playerPosition.X > 0) ? -speedToReduce : speedToReduce;

            float speedX = 0;

            if (speedToReduceInX >= 0)
            {
                if (AtMiddle() == false)
                {
                    if (direction.X >= 4)
                    {
                        speedX = -speedToReduceInX;
                        ChangeAtMiddle(true);
                    }
                    else
                        speedX = speedToReduceInX;
                }
                else
                {
                    if (direction.X <= 0)
                    {
                        speedX = 0;
                        ChangeAtMiddle(false);
                    }
                    else
                        speedX = -speedToReduceInX;
                }
            }
            else if (speedToReduceInX < 0)
            {
                if (AtMiddle() == false)
                {
                    if (direction.X <= -4)
                    {
                        speedX = -speedToReduceInX;
                        ChangeAtMiddle(true);
                    }
                    else
                        speedX = speedToReduceInX;
                }
                else
                {
                    if (direction.X >= 0)
                    {
                        speedX = 0;
                        ChangeAtMiddle(false);
                    }
                    else
                        speedX = -speedToReduceInX;
                }
            }

            if (direction.Z < -GetOriginalSpeed())
            {
                direction.Z = -GetOriginalSpeed();
                speedToReduce = 0;
                speedX = 0;
                ChangeDoTurnAround();
                this.rotation *= Matrix.CreateFromYawPitchRoll((float)Math.PI, 0, 0);
                SetOriginalPosition(Position);
            }
            
            float enemySpeedAfterChange = direction.X + speedX;
            float enemyZPositionAfterMoving = (Position.Z + direction.Z - speedToReduce);

            float enemyXPositionAfterMoving = Position.X + enemySpeedAfterChange;
            float xDifference = (float)(enemyXPositionAfterMoving - Position.X);
            float zDifference = (float)(enemyZPositionAfterMoving - Position.Z);

            Vector3 flyDirection = new Vector3(xDifference, 0, zDifference);
            setDirection(flyDirection);
        }

        public int GetHealth()
        {
            return this.health;
        }

        public void DeductHealth()
        {
            this.health -= 10;
        }

        public void CalculateEnemyTurnAround(Random rnd)
        {
            if (!doTurnAround && rnd.Next(enemyTurnAroundFactor) == 0)
            {                
                ChangeDoTurnAround();
            }
        }

        private bool CanAttackPlayerInRange(Vector3 playerPosition)
        {
            return Math.Abs(Position.Z - playerPosition.Z) <= enemyAttackRange &&
                    Math.Abs(Position.X - playerPosition.X) <= enemyAttackRange;
        }

        public bool canEnemyAttack(GameTime gameTime, Random rnd, Vector3 playerPosition)
        {
            if (CanAttackPlayerInRange(playerPosition))
            {                
                timeSinceLastShoot += gameTime.ElapsedGameTime.Milliseconds;
                return (timeSinceLastShoot >= enemyShootCd && rnd.Next(enemyAttackChanceFactor) == 0);
            }
            return false;
        }
        
        public Vector3 getAttackDirection(Random rnd, Vector3 playerPosition)
        {
            float attackXDeviation = getAttackDevaition(rnd);
            float attackYDeviation = getAttackDevaition(rnd);
            float attackZDeviation = getAttackDevaition(rnd);
            float xDifference = (float)(playerPosition.X - Position.X) + attackXDeviation;
            float yDifference = (float)(playerPosition.Y - Position.Y) + attackYDeviation;
            float zDifference = (float)(playerPosition.Z - Position.Z) + attackZDeviation;

            float distance = (float)Math.Sqrt((double)(xDifference * xDifference + yDifference * yDifference + zDifference * zDifference));
            float time = (distance / enemyBulletSpeed);
            Vector3 attackDirection = new Vector3(xDifference / time, yDifference / time, zDifference / time);
            timeSinceLastShoot = 0;
            return attackDirection;
        }

        public override void Draw(Matrix viewMatrix, Matrix projectionMatrix)
        {
            model.CopyAbsoluteBoneTransformsTo(modelTransforms);
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World = rotation * modelTransforms[mesh.ParentBone.Index] * worldMatrix;
                    effect.View = viewMatrix;
                    effect.Projection = projectionMatrix;
                }
                mesh.Draw();
            }
        }

        private float getAttackDevaition(Random rnd)
        {
            int rndNo = rnd.Next(enemyAttackDeviationFactor);
            switch (rndNo)
            {
                case 0:
                    return (float)(rnd.NextDouble() * deviationRange);
                case 1:
                    return (float)(rnd.NextDouble() * deviationRange / 2);
                case 2:
                    return (float)(rnd.NextDouble() * deviationRange / 4);
                default:
                    return 0;
            }
        }

        public bool CollidesWithRay(Ray ray)
        {
            float? collisionPosition;
            foreach (ModelMesh myModelMeshes in model.Meshes)
            {
                collisionPosition = ray.Intersects(myModelMeshes.BoundingSphere.Transform(worldMatrix));

                if (null != collisionPosition)
                    return true;
            }
            return false;
        }
    }
}
