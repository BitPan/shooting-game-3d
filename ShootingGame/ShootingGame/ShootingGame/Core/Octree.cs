﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using ShootingGame.Models;
using Microsoft.Xna.Framework.Graphics;

namespace ShootingGame.Core
{
    public class Octree
    {
        OcTreeNode octreeRoot;
        List<int> enemyIDs;
        List<int> playerIDs;
        List<int> enemyBulletIDs;
        List<int> playerBulletIDs;
        DrawableModel player;
        private int boundryLeft;
        private int boundryRight;
        private int boundryNear;
        private int boundryFar;
        

        private Model playerModel;
        private Model enemyPlaneModel;
        private Model playerBulletModel;
        private Model enemyBulletModel;


        public Octree(Vector3 position, int size, int[] worldData)
        {
            octreeRoot = new OcTreeNode(position, size);
            octreeRoot.RootSize = size;
            boundryLeft = worldData[0];
            boundryRight = worldData[1];
            boundryNear = worldData[2];
            boundryFar = worldData[3];
            enemyIDs = new List<int>();
            playerIDs = new List<int>();
            enemyBulletIDs = new List<int>();
            playerBulletIDs = new List<int>();
        }

        public void TestInitialize(Random rnd, int[] enemyData)
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

        public void Update(GameTime gameTime, Random rnd, FirstPersonCamera camera)
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
                }
                else
                octreeRoot.Add(model);
            }

            //Detect Collision
            List<int> modelsToRemove = new List<int>();
            octreeRoot.DetectCollision(ref modelsToRemove);

            foreach (int modelID in modelsToRemove)
                octreeRoot.RemoveDrawableModel(modelID);
        }

        public void UpdatePlayer(Vector3 cameraPosition)
        {
            //player.Position = cameraPosition;
            //int newID = octreeRoot.UpdateModelWorldMatrix(playerIDs[0], player);
            //playerIDs[0] = newID;
            //Console.WriteLine("player:" + playerIDs[0]);
            //DrawableModel playeroctreeRoot.UpdateModelWorldMatrix(playerIDs[0]);
        }


        public void LoadModels(List<Model> models)
        {
            playerModel = models[0];
            enemyPlaneModel = models[1];
            playerBulletModel = models[2];
            enemyBulletModel = models[3];
        }

        public void AddPlayerModel(Vector3 position)
        {
            DrawableModel newDModel = new Player(playerModel, Matrix.CreateScale(0.3f)*Matrix.CreateTranslation(position.X, position.Y, position.Z), Vector3.Zero);
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
    }
}
