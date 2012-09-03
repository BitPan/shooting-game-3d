using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ShootingGame
{
    class SpinningModel : BasicModel
    {
        Matrix rotation = Matrix.Identity;
        float yawAngle = 0;
        float pitchAngle = 0;
        float rollAngle = 0;
        float originalSpeed = 0;
        Vector3 originalPosition;
        bool doTurnAround;
        bool turnToMiddle;

        public SpinningModel(Model m, Vector3 Position, Vector3 Direction, float yaw, float pitch, float roll)
            : base(m)
        {
            world = Matrix.CreateTranslation(Position);
            yawAngle = yaw;
            pitchAngle = pitch;
            rollAngle = roll;
            direction = Direction;
            originalPosition = Position;
            originalSpeed = Direction.Z;
            doTurnAround = false;
            turnToMiddle = false;
        }

        public SpinningModel(float a, Model m, Vector3 Position, Vector3 Direction, float yaw, float pitch, float roll)
            : base(m)
        {
            world = Matrix.CreateScale(a) * Matrix.CreateTranslation(Position);
            yawAngle = yaw;
            pitchAngle = pitch;
            rollAngle = roll;
            direction = Direction;
            originalPosition = Position;
            originalSpeed = Direction.Z;
            rotation *= Matrix.CreateFromYawPitchRoll((float)Math.PI / 2f, pitchAngle, rollAngle);
        }

        public void setDirection(Vector3 direction)
        {
            this.direction = direction;
        }

        public override void Update()
        {

            originalPosition += Vector3.Forward * 1.65f;
            originalPosition += Vector3.UnitY * -0.75f;
            originalPosition += Vector3.UnitX * 0.45f;
            rotation *= Matrix.CreateFromYawPitchRoll(yawAngle, pitchAngle, rollAngle);
            world *= Matrix.CreateTranslation(direction);
        }

        public bool TurnAround()
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

        public override Matrix GetWorld()
        {
            return rotation * world;
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

    }
}
