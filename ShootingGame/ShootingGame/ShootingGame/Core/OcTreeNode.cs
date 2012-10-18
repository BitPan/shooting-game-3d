﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ShootingGame.Models;
using ShootingGame.GameUtil;

namespace ShootingGame.Core
{
    public class OcTreeNode
    {
        private const int maxObjectsInNode = 5;
        private const float minSize = 10.0f;

        private Vector3 center;
        private float size;
        List<DrawableModel> modelList;
        private BoundingBox nodeBoundingBox;

        OcTreeNode nodeUFL;
        OcTreeNode nodeUFR;
        OcTreeNode nodeUBL;
        OcTreeNode nodeUBR;
        OcTreeNode nodeDFL;
        OcTreeNode nodeDFR;
        OcTreeNode nodeDBL;
        OcTreeNode nodeDBR;
        List<OcTreeNode> childList;

        public static int modelsDrawn;
        private static int modelsStoredInQuadTree;

        public int ModelsDrawn { get { return modelsDrawn; } set { modelsDrawn = value; } }

        public OcTreeNode(Vector3 center, float size)
        {
            this.center = center;
            this.size = size;
            modelList = new List<DrawableModel>();
            childList = new List<OcTreeNode>(8);

            Vector3 diagonalVector = new Vector3(size / 2.0f, size / 2.0f, size / 2.0f);
            nodeBoundingBox = new BoundingBox(center - diagonalVector, center + diagonalVector);
        }

        public int Add(DrawableModel model)
        {
            model.ModelID = modelsStoredInQuadTree++;
            AddDrawableModel(model);
            return model.ModelID;
        }

        public DrawableModel UpdateModelWorldMatrix(int modelID)
        {
            DrawableModel deletedModel = RemoveDrawableModel(modelID);
            deletedModel.Update();
            AddDrawableModel(deletedModel);
            return deletedModel;
        }

        public void Update()
        {
            List<DrawableModel> newModels = new List<DrawableModel>();
            for (int i = 0; i < modelList.Count; i++ )
            {
                DrawableModel deletedModel = modelList[i];
                modelList.Remove(deletedModel);
                i--;
                deletedModel.Update();
                newModels.Add(deletedModel);                
            }
            foreach (DrawableModel model in newModels)
                AddDrawableModel(model);

            foreach (OcTreeNode node in childList)
                node.Update();
        }

        public DrawableModel findModel(int modelID)
        {
            DrawableModel dModel = null;
            for (int index = 0; index < modelList.Count; index++)
            {
                if (modelList[index].ModelID == modelID)
                {
                    dModel = modelList[index];
                    return dModel;
                }
            }
            return null;
        }

        public DrawableModel RemoveDrawableModel(int modelID)
        {
            DrawableModel dModel = findModel(modelID);
            int child = 0;

            if (null != dModel)
                modelList.Remove(dModel);
            
            while ((dModel == null) && (child < childList.Count))
            {
                dModel = childList[child++].RemoveDrawableModel(modelID);
            }

            return dModel;
        }
        
        public void DetectCollision()
        {
            List<EnemyPlane> enemyModel = new List<EnemyPlane>();
            List<Bullet> playerBullet = new List<Bullet>();

            foreach(DrawableModel model in modelList)
            {
                if (model.GetType().ToString().Equals("ShootingGame.EnemyPlane"))
                    enemyModel.Add((EnemyPlane)model);
                else if (model.GetType().ToString().Equals("ShootingGame.Bullet"))
                    playerBullet.Add((Bullet)model);                   
            }

            if(enemyModel.Count > 0 && playerBullet.Count > 0)
            {
                foreach(EnemyPlane enemy in enemyModel)
                {
                    foreach(Bullet bullet in playerBullet)
                    {
                        if(enemy.CollidesWith(bullet.Model, bullet.WorldMatrix))
                        {
                            modelList.Remove(enemy);
                            modelList.Remove(bullet);
                        }
                    }
                }
            }

            foreach (OcTreeNode childNode in childList)
            {
                childNode.DetectCollision();
            }
        }
        
        private void AddDrawableModel(DrawableModel dModel)
        {
            if (childList.Count == 0)
            {
                modelList.Add(dModel);

                bool maxObjectsReached = (modelList.Count > maxObjectsInNode);
                bool minSizeNotReached = (size > minSize);
                if (maxObjectsReached && minSizeNotReached)
                {
                    CreateChildNodes();
                    foreach (DrawableModel currentDModel in modelList)
                    {
                        Distribute(currentDModel);
                    }
                    modelList.Clear();
                }
            }
            else
            {
                Distribute(dModel);
            }
        }

        public bool CleanUpChildNodes()
        {
            for (int childNodeIndex = childList.Count - 1; childNodeIndex >= 0; childNodeIndex--)
            {
                if (childList[childNodeIndex].CleanUpChildNodes())
                {
                    childList.Remove(childList[childNodeIndex]);
                }
            }

            if (modelList.Count > 0)
                return false;

            if (childList.Count > 0)
                return false;

            //No content and all subnodes have been removed
            return true;
        }

        private void CreateChildNodes()
        {
            float sizeOver2 = size / 2.0f;
            float sizeOver4 = size / 4.0f;

            nodeUFR = new OcTreeNode(center + new Vector3(sizeOver4, sizeOver4, -sizeOver4), sizeOver2);
            nodeUFL = new OcTreeNode(center + new Vector3(-sizeOver4, sizeOver4, -sizeOver4), sizeOver2);
            nodeUBR = new OcTreeNode(center + new Vector3(sizeOver4, sizeOver4, sizeOver4), sizeOver2);
            nodeUBL = new OcTreeNode(center + new Vector3(-sizeOver4, sizeOver4, sizeOver4), sizeOver2);
            nodeDFR = new OcTreeNode(center + new Vector3(sizeOver4, -sizeOver4, -sizeOver4), sizeOver2);
            nodeDFL = new OcTreeNode(center + new Vector3(-sizeOver4, -sizeOver4, -sizeOver4), sizeOver2);
            nodeDBR = new OcTreeNode(center + new Vector3(sizeOver4, -sizeOver4, sizeOver4), sizeOver2);
            nodeDBL = new OcTreeNode(center + new Vector3(-sizeOver4, -sizeOver4, sizeOver4), sizeOver2);

            childList.Add(nodeUFR);
            childList.Add(nodeUFL);
            childList.Add(nodeUBR);
            childList.Add(nodeUBL);
            childList.Add(nodeDFR);
            childList.Add(nodeDFL);
            childList.Add(nodeDBR);
            childList.Add(nodeDBL);
        }

        private void Distribute(DrawableModel dModel)
        {
            Vector3 position = dModel.Position;
            if (position.Y > center.Y)          //Up
                if (position.Z < center.Z)      //Forward
                    if (position.X < center.X)  //Left
                        nodeUFL.AddDrawableModel(dModel);
                    else                        //Right
                        nodeUFR.AddDrawableModel(dModel);
                else                            //Back
                    if (position.X < center.X)  //Left
                        nodeUBL.AddDrawableModel(dModel);
                    else                        //Right
                        nodeUBR.AddDrawableModel(dModel);
            else                                //Down
                if (position.Z < center.Z)      //Forward
                    if (position.X < center.X)  //Left
                        nodeDFL.AddDrawableModel(dModel);
                    else                        //Right
                        nodeDFR.AddDrawableModel(dModel);
                else                            //Back
                    if (position.X < center.X)  //Left
                        nodeDBL.AddDrawableModel(dModel);
                    else                        //Right
                        nodeDBR.AddDrawableModel(dModel);
        }

        public void Draw(Matrix viewMatrix, Matrix projectionMatrix, BoundingFrustum cameraFrustrum)
        {
            bool intersected = cameraFrustrum.Intersects(nodeBoundingBox);
            //ContainmentType cameraNodeContainment = cameraFrustrum.Contains(nodeBoundingBox);
            //if (cameraNodeContainment != ContainmentType.Disjoint)
            if (intersected)
            {
                foreach (DrawableModel dModel in modelList)
                {
                    dModel.Draw(viewMatrix, projectionMatrix);
                    modelsDrawn++;
                }

                foreach (OcTreeNode childNode in childList)
                    childNode.Draw(viewMatrix, projectionMatrix, cameraFrustrum);
            }
        }

        public void DrawBoxLines(Matrix viewMatrix, Matrix projectionMatrix, GraphicsDevice device, BasicEffect basicEffect)
        {
            foreach (OcTreeNode childNode in childList)
                childNode.DrawBoxLines(viewMatrix, projectionMatrix, device, basicEffect);

            if (childList.Count == 0)
                XNAUtils.DrawBoundingBox(nodeBoundingBox, device, basicEffect, Matrix.Identity, viewMatrix, projectionMatrix);
        }
    }
}
