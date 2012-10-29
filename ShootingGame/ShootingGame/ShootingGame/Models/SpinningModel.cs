using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ShootingGame
{
    public class SpinningModel : BasicModel
    {
        Matrix rotation = Matrix.Identity;
        float yawAngle = 0;
        float pitchAngle = 0;
        float rollAngle = 0;
        float originalSpeed = 0;
        Vector3 originalPosition;
        bool doTurnAround;
        bool turnToMiddle;

        public SpinningModel(Model m, float scale, Vector3 Position, Vector3 Direction, float yaw, float pitch, float roll)
            : base(m)
        {
            world = Matrix.CreateScale(scale) * Matrix.CreateTranslation(Position);
            yawAngle = yaw;
            pitchAngle = pitch;
            rollAngle = roll;
            direction = Direction;
            originalPosition = Position;
            originalSpeed = Direction.Z;
            rotation *= Matrix.CreateFromYawPitchRoll((float)Math.PI / 2f, pitchAngle, rollAngle);
        }

        public void DoTranslation(Vector3 translation)
        {
            world *= Matrix.CreateTranslation(translation);
        }

        public void yawRotate(float rawRotate)
        {
            rotation *= Matrix.CreateFromYawPitchRoll(rawRotate, pitchAngle, rollAngle);
        }

        public void setDirection(Vector3 direction)
        {
            this.direction = direction;
        }

        public override void Update()
        {
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

        public void SetOriginalPosition(Vector3 position)
        {
            originalPosition = position;
        }


       
    }
}
