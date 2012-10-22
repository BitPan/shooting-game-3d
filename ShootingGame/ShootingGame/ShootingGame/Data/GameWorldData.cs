using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace ShootingGame.Data
{
    public class GameWorldData
    {
        private const int boundryLeft = -1000;

        public int BoundryLeft
        {
            get { return boundryLeft; }
        }


        private const int boundryRight = 1000;

        public int BoundryRight
        {
            get { return boundryRight; }
        }

        private const int boundryNear = 1000;

        public int BoundryNear
        {
            get { return boundryNear; }
        }

        private const int boundryFar = -1000;

        public int BoundryFar
        {
            get { return boundryFar; }
        }

        private const int FLYING_OUT_ZONE = 500;

        public int FLYING_OUT_ZONE1
        {
            get { return FLYING_OUT_ZONE; }
        }

        private const int PLAYER_BULLET_SPEED = 20;

        public int PLAYER_BULLET_SPEED1
        {
            get { return PLAYER_BULLET_SPEED; }
        }

        private Vector3 OCTREE_WORLD_CENTER = new Vector3(500, 0, -300);

        public Vector3 OCTREE_WORLD_CENTER1
        {
            get { return OCTREE_WORLD_CENTER; }
        }

        private const int OCTREE_WORLD_SIZE = 2000;

        public int OCTREE_WORLD_SIZE1
        {
            get { return OCTREE_WORLD_SIZE; }
        } 





    }
}
