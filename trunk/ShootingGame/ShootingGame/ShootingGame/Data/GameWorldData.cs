using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using GameData;

namespace ShootingGame.Data
{
    public class GameWorldData
    {

        public GameWorldData(WorldMatrix matrix) {
            this.boundryLeft = matrix.boundryLeft;
            this.boundryRight = matrix.boundryRight;
            this.boundryNear = matrix.boundryNear;
            this.boundryFar = matrix.boundryFar;
            this.FLYING_OUT_ZONE = matrix.FLYING_OUT_ZONE;
            this.PLAYER_BULLET_SPEED = matrix.PLAYER_BULLET_SPEED;
            this.OCTREE_WORLD_CENTER = new Vector3(matrix.OCTREE_WORLD_CENTER_X, 0, matrix.OCTREE_WORLD_CENTER_Z);
            this.OCTREE_WORLD_SIZE = matrix.OCTREE_WORLD_SIZE;
        }


        private  int boundryLeft ;

        public int BoundryLeft
        {
            get { return boundryLeft; }
        }


        private int boundryRight ;

        public int BoundryRight
        {
            get { return boundryRight; }
        }

        private  int boundryNear;

        public int BoundryNear
        {
            get { return boundryNear; }
        }

        private  int boundryFar ;

        public int BoundryFar
        {
            get { return boundryFar; }
        }

        private  int FLYING_OUT_ZONE ;

        public int FLYING_OUT_ZONE1
        {
            get { return FLYING_OUT_ZONE; }
        }

        private int PLAYER_BULLET_SPEED;

        public int PLAYER_BULLET_SPEED1
        {
            get { return PLAYER_BULLET_SPEED; }
        }

        private Vector3 OCTREE_WORLD_CENTER ;

        public Vector3 OCTREE_WORLD_CENTER1
        {
            get { return OCTREE_WORLD_CENTER; }
        }

        private  int OCTREE_WORLD_SIZE;

        public int OCTREE_WORLD_SIZE1
        {
            get { return OCTREE_WORLD_SIZE; }
        } 




    }
}
