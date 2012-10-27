using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameData
{
    [Serializable]
   public  class WorldMatrix
    {
        public int boundryLeft;
        public int boundryRight;
        public int boundryNear;
        public int boundryFar;
        public int FLYING_OUT_ZONE;
        public int PLAYER_BULLET_SPEED;
        public float OCTREE_WORLD_CENTER_X;
        public float OCTREE_WORLD_CENTER_Z;
        public int OCTREE_WORLD_SIZE;
    }
}
