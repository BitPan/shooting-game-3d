﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ShootingGame.Models;

namespace ShootingGame
{
    public class Bullet : DrawableModel
    {

        public Bullet(Model inModel, Matrix inWorldMatrix, Vector3 newDirection)
            : base(inModel, inWorldMatrix, newDirection)
        {
            worldMatrix = inWorldMatrix;
            this.direction = newDirection;
        }

        public void DoTranslation(Vector3 translation)
        {
            worldMatrix *= Matrix.CreateTranslation(translation);
        }

        public void setDirection(Vector3 direction)
        {
            this.direction = direction;
        }

        public override void Update()
        {
            worldMatrix *= Matrix.CreateTranslation(direction);
        }

        public Vector3 GetDirection()
        {
            return direction;
        }
    }
}
