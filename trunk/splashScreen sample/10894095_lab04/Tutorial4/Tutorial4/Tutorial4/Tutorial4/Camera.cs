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
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using System.Timers;


namespace Tutorial4
{

    public class Camera : Microsoft.Xna.Framework.GameComponent
    {
        //Camera matrices
        public Matrix view { get; protected set; }
        public Matrix projection { get; protected set; }

        // Camera vectors
        public Vector3 cameraPosition;
        public Vector3 cameraDirection;
        Vector3 cameraUp;

        // Speed
        float speed = 3;

        // Mouse stuff
        MouseState prevMouseState;
        KeyboardState prevKeyboardState;

        public Camera(Game game, Vector3 pos, Vector3 target, Vector3 up)
            : base(game)
        {
            // Build camera view matrix
            cameraPosition = pos;
            cameraDirection = target - pos;
            cameraDirection.Normalize();
            cameraUp = up;
            CreateLookAt();


            projection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.PiOver4,
                (float)Game.Window.ClientBounds.Width /
                (float)Game.Window.ClientBounds.Height,
                0.1f, 3000);
        }

        public override void Initialize()
        {
            // Set mouse position and do initial get state
            Mouse.SetPosition(Game.Window.ClientBounds.Width / 2,
                Game.Window.ClientBounds.Height / 2);
            prevMouseState = Mouse.GetState();
            prevKeyboardState = Keyboard.GetState();
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            // Move forward/backward
            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {

                cameraPosition += cameraDirection * speed;

            }
            if (Keyboard.GetState().IsKeyDown(Keys.S))
                cameraPosition -= cameraDirection * speed;
            // Move side to side
            if (Keyboard.GetState().IsKeyDown(Keys.A))
                cameraPosition += Vector3.Cross(cameraUp, cameraDirection) * speed;
            if (Keyboard.GetState().IsKeyDown(Keys.D))
                cameraPosition -= Vector3.Cross(cameraUp, cameraDirection) * speed;
            
            // Yaw rotation
            cameraDirection = Vector3.Transform(cameraDirection,
                //Matrix.CreateRotationX((-MathHelper.PiOver4 / 150) *
                // (Mouse.GetState().X - prevMouseState.X)));
                Matrix.CreateFromAxisAngle(cameraUp, (-MathHelper.PiOver4 / 150) *
                (Mouse.GetState().X - prevMouseState.X)));

            // Roll rotation
            //if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            //{
            //    cameraUp = Vector3.Transform(cameraUp,
            //        Matrix.CreateFromAxisAngle(cameraDirection,
            //        MathHelper.PiOver4 / 45));
            // }
            // if (Mouse.GetState().RightButton == ButtonState.Pressed)
            // {
            //    cameraUp = Vector3.Transform(cameraUp,
            //        Matrix.CreateFromAxisAngle(cameraDirection,
            //        -MathHelper.PiOver4 / 45));
            //}

            // Pitch rotation
            cameraDirection = Vector3.Transform(cameraDirection,
                Matrix.CreateFromAxisAngle(Vector3.Cross(cameraUp, cameraDirection),
                (MathHelper.PiOver4 / 100) *
                (Mouse.GetState().Y - prevMouseState.Y)));

            
            if (prevKeyboardState.IsKeyDown(Keys.Space))
                cameraPosition -= new Vector3(0, 200, 0);
            //cameraUp = Vector3.Transform(cameraUp,
            // Matrix.CreateFromAxisAngle(Vector3.Cross(cameraUp, cameraDirection),
            //(MathHelper.PiOver4 / 100) *
            //(Mouse.GetState().Y - prevMouseState.Y)));

            // Reset prevMouseState
            prevMouseState = Mouse.GetState();
            prevKeyboardState = Keyboard.GetState();

            if (cameraPosition.Y <= 13)
                cameraPosition.Y = 13;
            if (cameraPosition.Y > 13)
                cameraPosition.Y = 13;

            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                cameraPosition += new Vector3(0, 200, 0);

            }
            // Recreate the camera view matrix
            CreateLookAt();

            base.Update(gameTime);
        }

        private void CreateLookAt()
        {
            view = Matrix.CreateLookAt(cameraPosition,
                cameraPosition + cameraDirection, cameraUp);
            //System.Diagnostics.Debug.WriteLine(cameraPosition + cameraDirection);
        }

    }
}