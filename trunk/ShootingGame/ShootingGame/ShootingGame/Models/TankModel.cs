using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ShootingGame.Models;
using ShootingGame.GameUtil;

namespace ShootingGame
{
    public class TankModel : DrawableModel
    {
        #region Fields


        // The XNA framework Model object that we are going to display.
        private bool isMoving;
        private double currentTurnedAngle;
        private Mode currentState;
        private int[,] cityMap;
        private Vector3 targetDestination;
        private bool enableAutoSearch;
        private LinkedList<Vector2> pathToTargetDestination;
        private PathFinder finder;
        private bool cannonMovingUp;

        public enum Mode{
            WANDER,
            FOLLOW,
            ATTACK,
            STOP
        }

        // Shortcut references to the bones that we are going to animate.
        // We could just look these up inside the Draw method, but it is more
        // efficient to do the lookups while loading and cache the results.
        ModelBone leftBackWheelBone;
        ModelBone rightBackWheelBone;
        ModelBone leftFrontWheelBone;
        ModelBone rightFrontWheelBone;
        ModelBone leftSteerBone;
        ModelBone rightSteerBone;
        ModelBone turretBone;
        ModelBone cannonBone;
        ModelBone hatchBone;


        // Store the original transform matrix for each animating bone.
        Matrix leftBackWheelTransform;
        Matrix rightBackWheelTransform;
        Matrix leftFrontWheelTransform;
        Matrix rightFrontWheelTransform;
        Matrix leftSteerTransform;
        Matrix rightSteerTransform;
        Matrix turretTransform;
        Matrix cannonTransform;
        Matrix hatchTransform;


        // Array holding all the bone transform matrices for the entire model.
        // We could just allocate this locally inside the Draw method, but it
        // is more efficient to reuse a single array, as this avoids creating
        // unnecessary garbage.

        // Current animation positions.
        float wheelRotationValue;
        float steerRotationValue;
        float turretRotationValue;
        float cannonRotationValue;
        float hatchRotationValue;


        #endregion

        #region Properties


        /// <summary>
        /// Gets or sets the wheel rotation amount.
        /// </summary>
        public float WheelRotation
        {
            get { return wheelRotationValue; }
            set { wheelRotationValue = value; }
        }


        /// <summary>
        /// Gets or sets the steering rotation amount.
        /// </summary>
        public float SteerRotation
        {
            get { return steerRotationValue; }
            set { steerRotationValue = value; }
        }


        /// <summary>
        /// Gets or sets the turret rotation amount.
        /// </summary>
        public float TurretRotation
        {
            get { return turretRotationValue; }
            set { turretRotationValue = value; }
        }


        /// <summary>
        /// Gets or sets the cannon rotation amount.
        /// </summary>
        public float CannonRotation
        {
            get { return cannonRotationValue; }
            set { cannonRotationValue = value; }
        }


        /// <summary>
        /// Gets or sets the entry hatch rotation amount.
        /// </summary>
        public float HatchRotation
        {
            get { return hatchRotationValue; }
            set { hatchRotationValue = value; }
        }


        #endregion        

        public TankModel(Model inModel, Matrix inWorldMatrix, Vector3 newDirection, int[,] cityMap)
            : base(inModel, inWorldMatrix, newDirection)
        {
            leftBackWheelBone = inModel.Bones["l_back_wheel_geo"];
            rightBackWheelBone = inModel.Bones["r_back_wheel_geo"];
            leftFrontWheelBone = inModel.Bones["l_front_wheel_geo"];
            rightFrontWheelBone = inModel.Bones["r_front_wheel_geo"];
            leftSteerBone = inModel.Bones["l_steer_geo"];
            rightSteerBone = inModel.Bones["r_steer_geo"];
            turretBone = inModel.Bones["turret_geo"];
            cannonBone = inModel.Bones["canon_geo"];
            hatchBone = inModel.Bones["hatch_geo"];

            // Store the original transform matrix for each animating bone.
            leftBackWheelTransform = leftBackWheelBone.Transform;
            rightBackWheelTransform = rightBackWheelBone.Transform;
            leftFrontWheelTransform = leftFrontWheelBone.Transform;
            rightFrontWheelTransform = rightFrontWheelBone.Transform;
            leftSteerTransform = leftSteerBone.Transform;
            rightSteerTransform = rightSteerBone.Transform;
            turretTransform = turretBone.Transform;
            cannonTransform = cannonBone.Transform;
            hatchTransform = hatchBone.Transform;

            // Allocate the transform matrix array.
            worldMatrix = inWorldMatrix;
            this.direction = newDirection;
            this.cityMap = cityMap;
            //targetDestination = new Vector3(700,0,-700);
            finder = new PathFinder();
            finder.SetUpMap(cityMap);
            pathToTargetDestination = new LinkedList<Vector2>();
            isMoving = false;
            currentTurnedAngle = 0;
            ActivateWanderMode();
            cannonMovingUp = true;
        }

        private void EnableAutoSearch()
        {
            this.enableAutoSearch = true;
        }

        private void DisableAutoSearch()
        {
            this.enableAutoSearch = false;
        }

        public void ActivateWanderMode()
        {
            EnableAutoSearch();
            this.currentState = Mode.WANDER;
        }

        public void ActivateFollowMode()
        {
            pathToTargetDestination.Clear();
            this.currentState = Mode.FOLLOW;
        }

        public void ActivateAttackMode()
        {
            this.enableAutoSearch = false;
            this.currentState = Mode.ATTACK;
        }

        public void DeactiveActionMode()
        {
            this.enableAutoSearch = false;
            this.currentState = Mode.STOP;
        }

        public void yawRotate(float rawRotate)
        {
            rotation *= Matrix.CreateFromYawPitchRoll(rawRotate, 0, 0);
        }

        public void DoTranslation(Vector3 translation)
        {
            worldMatrix *= Matrix.CreateTranslation(translation);
        }

        public void setDirection(Vector3 direction)
        {
            direction.Normalize();
            this.direction = direction;
        }

        public void SetDestination(Vector3 destination)
        {
            this.targetDestination = destination;
        }

        public Vector2 PickRandomPosition(Random rnd)
        {
            int xIndex = rnd.Next(cityMap.GetLength(0));
            int yIndex = rnd.Next(cityMap.GetLength(1));
            if (cityMap[xIndex, yIndex] == 0)
                return new Vector2(xIndex, yIndex);
            else
                PickRandomPosition(rnd);
            return new Vector2(0,0);
        }

        public void Update(Player player, Random rnd)
        {
            if (this.currentState == Mode.WANDER)
            {
                if (enableAutoSearch)
                {
                    targetDestination = MapIndexToWorldCorrd(PickRandomPosition(rnd));
                    DisableAutoSearch();
                }
                StartMoving(player, rnd);
            }
            else if (this.currentState == Mode.FOLLOW)
            {
                targetDestination = player.Position;
                StartMoving(player, rnd);
            }
            else if (this.currentState == Mode.ATTACK)
            {
                UpdateCannonRotation();
                TurretRotation += 0.05f;
            }
            else if (this.currentState == Mode.STOP)
            {

            }
        }

        private void UpdateCannonRotation()
        {
            if (cannonMovingUp)
            {
                CannonRotation += 0.05f;
                if (CannonRotation >= 3f)
                    cannonMovingUp = false;
            }
            else
            {
                CannonRotation -= 0.05f;
                if (CannonRotation <= 0.3f)
                    cannonMovingUp = true;
            }
        }

        private void StartMoving(Player player, Random rnd)
        {
            if (isMoving)
            {
                WheelRotation += 0.05f;
            }
            if (Position != targetDestination)
            {
                if (Math.Abs(Position.X - targetDestination.X) <= 25 && Math.Abs(Position.Z - targetDestination.Z) <= 25)
                {
                    Position = targetDestination;
                    isMoving = false;
                    pathToTargetDestination.Clear();
                    if (this.currentState == Mode.WANDER)
                        EnableAutoSearch();
                    else if (this.currentState == Mode.FOLLOW)
                        DeactiveActionMode();
                }
                else
                {
                    if (null == pathToTargetDestination || pathToTargetDestination.Count == 0)
                        CalculatePath();
                    if (null != pathToTargetDestination && pathToTargetDestination.Count > 0)
                    {
                        Vector2 targetPoint = pathToTargetDestination.First();
                        Vector3 targetPoint1 = MapIndexToWorldCorrd(targetPoint);
                        if (Math.Abs(Position.X - targetPoint1.X) <= 25 && Math.Abs(Position.Z - targetPoint1.Z) <= 25)
                        {
                            Position = targetPoint1;
                            pathToTargetDestination.RemoveFirst();
                        }
                        else
                        {
                            double angleToTurn = MathUtil.AngleToTurn(Position, targetPoint1);
                            isMoving = true;
                            if (Math.Abs(currentTurnedAngle - angleToTurn) >= Math.PI / 50f)
                            {
                                if (currentTurnedAngle > angleToTurn)
                                {
                                    SteerRotation -= (float)Math.PI / 360f;
                                    yawRotate(-(float)Math.PI / 360f);
                                    currentTurnedAngle -= Math.PI / 360;

                                }
                                else
                                {
                                    SteerRotation += (float)Math.PI / 360f;
                                    yawRotate((float)Math.PI / 360f);
                                    currentTurnedAngle += Math.PI / 360;

                                }

                                if (Math.Abs(currentTurnedAngle - angleToTurn) <= Math.PI / 100f)
                                {
                                    SteerRotation = 0;
                                    yawRotate((float)(angleToTurn - currentTurnedAngle));
                                    currentTurnedAngle = angleToTurn;
                                }
                            }
                            else
                            {
                                if (CheckIsCollidingWithPlayer(Matrix.CreateTranslation(direction), player.Position))
                                    isMoving = false;
                                else
                                {
                                    isMoving = true;
                                    SteerRotation = 0;
                                    setDirection((targetPoint1 - position));
                                    WorldMatrix = worldMatrix * Matrix.CreateTranslation(direction);
                                }
                            }
                        }
                    }
                }
            }
        }

        private bool CheckIsCollidingWithPlayer(Matrix translation, Vector3 playerPosition)
        {
            Matrix  nextFrameWorldMatrix = WorldMatrix * translation;
            return DetectPlayerCollision(nextFrameWorldMatrix , playerPosition);
        }

        private bool IsTurnedToTarget(Vector3 targetPoint)
        {
            return true;
        }

        private void CalculatePath()
        {
            pathToTargetDestination = finder.FindPath(MapWorldCorrdToIndex(Position), MapWorldCorrdToIndex(targetDestination));
        }

        private Vector3 MapWorldCorrdToIndex(Vector3 worldCoord)
        {
            float xIndex = (worldCoord.X - worldCoord.X % 50)/50;
            float zIndex = -(worldCoord.Z - worldCoord.Z % 50)/50;
            return new Vector3(xIndex, 0 , zIndex);
        }

        private Vector3 MapIndexToWorldCorrd(Vector2 mapIndex)
        {
            float xCoord = mapIndex.X * 50 + 25;
            float zCoord = mapIndex.Y * 50 + 25;
            return new Vector3(xCoord, 0, -zCoord);
        }

        public Vector3 GetDirection()
        {
            return direction;
        }

        public bool DetectPlayerCollision(Vector3 playerPosition)
        {
            BoundingSphere playerSphere = new BoundingSphere(playerPosition, 10f);

            foreach (ModelMesh myModelMeshes in model.Meshes)
            {
                if (myModelMeshes.BoundingSphere.Transform(worldMatrix).Intersects(playerSphere))
                    return true;
            }
            return false;
        }

        public bool DetectPlayerCollision(Matrix newWorld, Vector3 playerPosition)
        {
            BoundingSphere playerSphere = new BoundingSphere(playerPosition, 10f);

            foreach (ModelMesh myModelMeshes in model.Meshes)
            {
                if (myModelMeshes.BoundingSphere.Transform(newWorld).Intersects(playerSphere))
                    return true;
            }
            return false;
        }

        public override void Draw(Matrix viewMatrix, Matrix projectionMatrix)
        {
            Matrix wheelRotation = Matrix.CreateRotationX(wheelRotationValue);
            Matrix steerRotation = Matrix.CreateRotationY(steerRotationValue);
            Matrix turretRotation = Matrix.CreateRotationY(turretRotationValue);
            Matrix cannonRotation = Matrix.CreateRotationX(cannonRotationValue);
            Matrix hatchRotation = Matrix.CreateRotationX(hatchRotationValue);

            // Apply matrices to the relevant bones.
            leftBackWheelBone.Transform = wheelRotation * leftBackWheelTransform;
            rightBackWheelBone.Transform = wheelRotation * rightBackWheelTransform;
            leftFrontWheelBone.Transform = wheelRotation * leftFrontWheelTransform;
            rightFrontWheelBone.Transform = wheelRotation * rightFrontWheelTransform;
            leftSteerBone.Transform = steerRotation * leftSteerTransform;
            rightSteerBone.Transform = steerRotation * rightSteerTransform;
            turretBone.Transform = turretRotation * turretTransform;
            cannonBone.Transform = cannonRotation * cannonTransform;
            hatchBone.Transform = hatchRotation * hatchTransform;

            // Look up combined bone matrices for the entire model.
            model.CopyAbsoluteBoneTransformsTo(modelTransforms);

            //Matrix tankview = Matrix.CreateLookAt(tankPosition, camera.cameraPosition+camera.cameraDirection, Vector3.Up);
            // Vector3 tankDirection = camera.cameraPosition + camera.cameraDirection;
            //System.Diagnostics.Debug.WriteLine("Tank"+ tankDirection);
            // Draw the model.


            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World =  modelTransforms[mesh.ParentBone.Index] *rotation * worldMatrix;
                    effect.View = viewMatrix;
                    effect.Projection = projectionMatrix;
                }
                mesh.Draw();
            }

        }
    }
}
