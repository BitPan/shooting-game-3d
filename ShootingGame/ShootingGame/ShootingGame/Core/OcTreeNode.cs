using System;
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
        private float rootSize;
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

        public float RootSize { get { return rootSize; } set { rootSize = value; } }
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

        public void UpdateModelWorldMatrix(int modelID, Vector3 newPosition)
        {
            //findModel(modelID).Position = newPosition;
            //AddDrawableModel(newModel);
            //return newModel.ModelID;
        }

        public void GetUpdatedModels(ref List<DrawableModel> models)
        {
            if (modelList.Count > 0)
            {
                for (int i = 0; i < modelList.Count; i++)
                {
                    DrawableModel deletedModel = modelList[i];
                    modelList.RemoveAt(i);
                    i--;
                    deletedModel.Update();
                    if (!IsModelOutOfOctree(deletedModel))
                        models.Add(deletedModel);
                }

            }
            else
            {
                foreach (OcTreeNode node in childList)
                    node.GetUpdatedModels(ref models);
            }
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

        public void DetectCollision(City city, ref List<int> modelsToRemove, ref List<int> bulletsToRemove)
        {
            List<EnemyPlane> enemyModel = new List<EnemyPlane>();
            List<Bullet> playerBullet = new List<Bullet>();
            List<EnemyBullet> enemyBullet = new List<EnemyBullet>();
            List<Player> player = new List<Player>();

            if (modelList.Count > 0)
            {
                foreach (DrawableModel model in modelList)
                {
                    if (model.GetType().ToString().Equals("ShootingGame.EnemyPlane"))
                        enemyModel.Add((EnemyPlane)model);
                    else if (model.GetType().ToString().Equals("ShootingGame.Bullet"))
                        playerBullet.Add((Bullet)model);
                    else if (model.GetType().ToString().Equals("ShootingGame.EnemyBullet"))
                        enemyBullet.Add((EnemyBullet)model);
                    else if (model.GetType().ToString().Equals("ShootingGame.Player"))
                        player.Add((Player)model);
                }
            }
            else
            {
                foreach (OcTreeNode childNode in childList)
                {
                    childNode.DetectCollision(city, ref modelsToRemove, ref bulletsToRemove);
                }
            }

            if (enemyBullet.Count > 0)
            {
                for (int i = 0; i < enemyBullet.Count; i++)
                {
                    if (city.CollideWithBullet(enemyBullet[i].Model, enemyBullet[i].WorldMatrix))
                    {
                        bulletsToRemove.Add(enemyBullet[i].ModelID);
                        enemyBullet.RemoveAt(i);
                        i = i > 0 ? i - 1 : 0;
                    }
                }
            }

            if (playerBullet.Count > 0)
            {
                for (int j = 0; j < playerBullet.Count; j++)
                {
                    if (city.CollideWithBullet(playerBullet[j].Model, playerBullet[j].WorldMatrix))
                    {
                        bulletsToRemove.Add(playerBullet[j].ModelID);
                        playerBullet.RemoveAt(j);
                        j = j > 0 ? j - 1 : 0;
                    }
                }
            }

            if (enemyModel.Count > 0 && playerBullet.Count > 0)
            {
                for (int i = 0; i < enemyModel.Count; i++)
                {
                    for (int j = 0; j < playerBullet.Count; j++)
                    {
                        if (enemyModel[i].CollidesWith(playerBullet[j].Model, playerBullet[j].WorldMatrix))
                        {
                            enemyModel[i].DeductHealth();
                            modelsToRemove.Add(playerBullet[j].ModelID);
                            playerBullet.RemoveAt(j);
                            j = j > 0 ? j - 1 : 0;
                            
                            if (enemyModel[i].GetHealth() <= 0)
                            {
                                modelsToRemove.Add(enemyModel[i].ModelID);
                                enemyModel.RemoveAt(i);
                                i = i > 0 ? i - 1 : 0;
                            }

                        }
                    }
                }
                enemyModel.Clear();
                playerBullet.Clear();
            }

            if (enemyBullet.Count > 0 && player.Count > 0)
            {
                for (int i = 0; i < enemyBullet.Count; i++)
                {
                    for (int j = 0; j < player.Count; j++)
                    {
                        if (enemyBullet[i].CollidesWithPlayer(player[j].Position))
                        {
                            modelsToRemove.Add(enemyBullet[i].ModelID);
                            enemyBullet.RemoveAt(i);
                            i = i > 0 ? i - 1 : 0;
                        }
                    }
                }
                enemyBullet.Clear();
                player.Clear();
            }

        }

        private bool IsModelOutOfOctree(DrawableModel dModel)
        {
            return Math.Abs(dModel.Position.X - center.X) > rootSize ||
                Math.Abs(dModel.Position.Y - center.Y) > rootSize ||
                Math.Abs(dModel.Position.Z - center.Z) > rootSize;
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

            nodeUFR.RootSize = rootSize;
            nodeUFL.RootSize = rootSize;
            nodeUBR.RootSize = rootSize;
            nodeUBL.RootSize = rootSize;
            nodeDFR.RootSize = rootSize;
            nodeDFL.RootSize = rootSize;
            nodeDBR.RootSize = rootSize;
            nodeDBL.RootSize = rootSize;

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
            //ContainmentType cameraNodeContainment = cameraFrustrum.Contains(nodeBoundingBox);
            //if (cameraNodeContainment != ContainmentType.Disjoint)
            //{
                foreach (DrawableModel dModel in modelList)
                {
                    dModel.Draw(viewMatrix, projectionMatrix);
                    modelsDrawn++;
                }

                foreach (OcTreeNode childNode in childList)
                    childNode.Draw(viewMatrix, projectionMatrix, cameraFrustrum);
          //  }
        }

        public void DrawBoxLines(Matrix viewMatrix, Matrix projectionMatrix, GraphicsDevice device, BasicEffect basicEffect)
        {
            foreach (OcTreeNode childNode in childList)
                childNode.DrawBoxLines(viewMatrix, projectionMatrix, device, basicEffect);

            if (modelList.Count > 0)
                XNAUtils.DrawBoundingBox(nodeBoundingBox, device, basicEffect, Matrix.Identity, viewMatrix, projectionMatrix);
        }
    }
}
