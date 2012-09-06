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

        private const int boundryLeft = -800;
        private const int boundryRight = 800;
        private const int boundryNear = 1500;
        private const int boundryFar = -1500;    

        float pitchAngle;
        float yawAngle;
        float totalYaw = MathHelper.PiOver4;
        float currentYaw = 0;
        float totalPitch = MathHelper.Pi;
        float currentPitch = 0;

        private static int MOUSE_SENSITY_FACTOR = 2;
        MouseState prevMouseState;
        private static readonly Random random = new Random();



        private bool shaking;
        private float shakeMagnitude;
        private float shakeDuration;
        private float shakeTimer;
        private Vector3 shakeOffset;
        private Vector3 originalDirection;

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
                cameraPostion = ProcessedTranslation(new Vector3(movingDistance, 0, movingDistance) * cameraDirection);
            if (keyState.IsKeyDown(Keys.S))
                cameraPostion = ProcessedTranslation(-new Vector3(movingDistance, 0, movingDistance) * cameraDirection);
            if (keyState.IsKeyDown(Keys.A))
                cameraPostion = ProcessedTranslation(Vector3.Cross(cameraUp, cameraDirection) * movingDistance);
            if (keyState.IsKeyDown(Keys.D))
                cameraPostion = ProcessedTranslation(-Vector3.Cross(cameraUp, cameraDirection) * movingDistance);

            yawAngle = (-MathHelper.PiOver4 / 150) * (Mouse.GetState().X - prevMouseState.X) * MOUSE_SENSITY_FACTOR;
            cameraDirection = Vector3.Transform(cameraDirection, Matrix.CreateFromAxisAngle(cameraUp, yawAngle));
            currentYaw += yawAngle;
             
             
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

            if (shaking)
                DoShake(gameTime);
            CreateLookAt();
            base.Update(gameTime);
        }

        private Vector3 ProcessedTranslation(Vector3 translation)
        {
            Vector3 preocessedPostion =  cameraPostion + translation;
            preocessedPostion.X = preocessedPostion.X >= boundryRight ? boundryRight : preocessedPostion.X;
            preocessedPostion.X = preocessedPostion.X <= boundryLeft ? boundryLeft : preocessedPostion.X;
            preocessedPostion.Z = preocessedPostion.Z >= boundryNear ? boundryNear : preocessedPostion.Z;
            preocessedPostion.Z = preocessedPostion.Z <= boundryFar ? boundryFar : preocessedPostion.Z;

            return preocessedPostion;
        }

        public void SetShake(float magnitude, float duration)
        {
            shaking = true;
            shakeMagnitude = magnitude;
            shakeDuration = duration;
            shakeTimer = 0f;
            originalDirection = cameraDirection;
        }

        /// <summary>
        /// Updates the Camera.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public void DoShake(GameTime gameTime)
        {
            shakeTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (shakeTimer >= shakeDuration)
            {
                shaking = false;
                shakeTimer = shakeDuration;
                cameraDirection = originalDirection;
            }

            float progress = shakeTimer / shakeDuration;
            float magnitude = shakeMagnitude * (1f - (progress * progress));

            shakeOffset = new Vector3(NextFloat(), NextFloat(), NextFloat()) * magnitude;

            cameraDirection += shakeOffset;
        }

        /// <summary>
        /// Helper to generate a random float in the range of [-1, 1].
        /// </summary>
        private float NextFloat()
        {
            return (float)random.NextDouble() * 2f - 1f;
        }

    }
}
