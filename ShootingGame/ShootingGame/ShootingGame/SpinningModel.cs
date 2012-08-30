using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ShootingGame
{
    class SpinningModel: BasicModel
    {
        Matrix rotation = Matrix.Identity;
        float yawAngle = 0;
        float pitchAngle = 0;
        float rollAngle = 0;
        float originalSpeed = 0;
        Vector3 originalPosition;

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
            rotation *= Matrix.CreateFromYawPitchRoll(yawAngle ,pitchAngle ,rollAngle);
            world *= Matrix.CreateTranslation(direction);
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
