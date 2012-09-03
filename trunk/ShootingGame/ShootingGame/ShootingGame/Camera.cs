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
    public class Camera : Microsoft.Xna.Framework.GameComponent
    {
        public Matrix view { get; protected set; }
        public Matrix projection { get; protected set; }

        public Vector3 cameraPostion { get; protected set; }
        public Vector3 cameraDirection;
        public Vector3 cameraUp;
        float pitchAngle;
        float yawAngle;
        private static int MOUSE_SENSITY_FACTOR = 2;

        MouseState prevMouseState;

        float totalYaw = MathHelper.PiOver4;
        float currentYaw = 0;
        float totalPitch = MathHelper.Pi;
        float currentPitch = 0;


        public Vector3 getCameraDirection()
        {
            return cameraDirection;
        }

        public void CreateLookAt()
        {
            view = Matrix.CreateLookAt(cameraPostion, cameraPostion +
                cameraDirection, cameraUp);
        }
        //构造函数
        public Camera(Game game, Vector3 pos, Vector3 target, Vector3 up)
            : base(game)
        {
            
            cameraPostion = pos;
            cameraDirection = target - pos;
            cameraDirection.Normalize();
            cameraUp = up;
            CreateLookAt();

            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
                (float)Game.Window.ClientBounds.Width /
                (float)Game.Window.ClientBounds.Height, 1, 10000);
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is wheadare it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here
            Mouse.SetPosition(Game.Window.ClientBounds.Width / 2,
                Game.Window.ClientBounds.Height / 2);
            prevMouseState = Mouse.GetState();
            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here
            KeyboardState keyState = Keyboard.GetState();
            float fps = (float)(1 / (gameTime.ElapsedGameTime.TotalMilliseconds / 1000));
            float movingDistance = 100 / fps;

            if (keyState.IsKeyDown(Keys.W))
                cameraPostion = cameraPostion + new Vector3(movingDistance, 0, movingDistance) * cameraDirection;
            if (keyState.IsKeyDown(Keys.S))
                cameraPostion = cameraPostion - new Vector3(movingDistance, 0, movingDistance) * cameraDirection;
            if (keyState.IsKeyDown(Keys.A))
                cameraPostion += Vector3.Cross(cameraUp, cameraDirection) * movingDistance;
            if (keyState.IsKeyDown(Keys.D)) 
                cameraPostion -= Vector3.Cross(cameraUp, cameraDirection) * movingDistance;

            //Yaw旋转
            yawAngle = (-MathHelper.PiOver4 / 150) * (Mouse.GetState().X - prevMouseState.X) * MOUSE_SENSITY_FACTOR;
            //if (Math.Abs(currentYaw + yawAngle) < totalYaw)
           // {
                cameraDirection = Vector3.Transform(cameraDirection, Matrix.CreateFromAxisAngle(cameraUp, yawAngle));
                currentYaw += yawAngle;
            //}
             
             
            //Pitch旋转
            pitchAngle = (MathHelper.PiOver4 / 150) *
                (Mouse.GetState().Y - prevMouseState.Y) * MOUSE_SENSITY_FACTOR;
            if (Math.Abs(currentPitch + pitchAngle) < totalPitch)
            {
                cameraDirection = Vector3.Transform(cameraDirection,
                    Matrix.CreateFromAxisAngle(Vector3.Cross(cameraUp, cameraDirection),
                   pitchAngle));
                currentPitch += pitchAngle;
            }
            prevMouseState = Mouse.GetState();
            CreateLookAt();
            base.Update(gameTime);
        }

        public Matrix WeaponWorldMatrix(float xOffset, float yOffset, float zOffset, float scale)
        {
            Vector3 weaponPos = cameraPostion;

            weaponPos += cameraDirection * zOffset;
            weaponPos += new Vector3(0,1,0) * yOffset;
            weaponPos += new Vector3(1,0,0) * xOffset;

            return Matrix.CreateRotationX(MathHelper.ToRadians(pitchAngle))
                    * Matrix.CreateRotationY(MathHelper.ToRadians(yawAngle))
                    * Matrix.CreateTranslation(weaponPos);
        }
    }
}
