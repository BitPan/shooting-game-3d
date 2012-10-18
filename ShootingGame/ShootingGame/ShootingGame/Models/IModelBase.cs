using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ShootingGame.Models
{
    interface IModelBase
    {
        void Draw(Matrix viewMatrix, Matrix projectionMatrix);
        bool CollidesWith(Model otherModel, Matrix otherWorld);
    }
}
