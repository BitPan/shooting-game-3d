using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace ShootingGame.GameUtil
{
    public class MathUtil
    {
        private static double DegreeToRadian(double angle) { return Math.PI * angle / 180.0; }
        private static double RadianToDegree(double angle) { return angle * (180.0 / Math.PI); }

        public static double AngleToTurn(Vector3 position, Vector3 target)
        {
            return (double)Math.Atan2((target.X - position.X), target.Z - position.Z);
        }

    }
}
