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


namespace ShootingGame.Data
{
    class CameraData
    {
        private const float CAMERA_FOVX = 85.0f;

        public float CAMERA_FOVX1
        {
            get { return CAMERA_FOVX; }
        }

        private const float WALL_HEIGHT = 800.0f;

        public float WALL_HEIGHT1
        {
            get { return WALL_HEIGHT; }
        }

        private const float CAMERA_ZNEAR = 0.1f;

        public float CAMERA_ZNEAR1
        {
            get { return CAMERA_ZNEAR; }
        }

        private const float FLOOR_PLANE_SIZE = 2000f;

        public float FLOOR_PLANE_SIZE1
        {
            get { return FLOOR_PLANE_SIZE; }
        }

        private const float CAMERA_ZFAR = FLOOR_PLANE_SIZE * 2.0f;

        public float CAMERA_ZFAR1
        {
            get { return CAMERA_ZFAR; }
        }

        private const float CAMERA_PLAYER_EYE_HEIGHT = 20.0f;

        public float CAMERA_PLAYER_EYE_HEIGHT1
        {
            get { return CAMERA_PLAYER_EYE_HEIGHT; }
        }

        private const float CAMERA_ACCELERATION_X = 800.0f;

        public float CAMERA_ACCELERATION_X1
        {
            get { return CAMERA_ACCELERATION_X; }
        }

        private const float CAMERA_ACCELERATION_Y = 800.0f;

        public float CAMERA_ACCELERATION_Y1
        {
            get { return CAMERA_ACCELERATION_Y; }
        }

        private const float CAMERA_ACCELERATION_Z = 800.0f;

        public float CAMERA_ACCELERATION_Z1
        {
            get { return CAMERA_ACCELERATION_Z; }
        }

        private const float CAMERA_VELOCITY_X = 50.0f;

        public float CAMERA_VELOCITY_X1
        {
            get { return CAMERA_VELOCITY_X; }
        }

        private const float CAMERA_VELOCITY_Y = 50.0f;

        public float CAMERA_VELOCITY_Y1
        {
            get { return CAMERA_VELOCITY_Y; }
        }

        private const float CAMERA_VELOCITY_Z = 50.0f;

        public float CAMERA_VELOCITY_Z1
        {
            get { return CAMERA_VELOCITY_Z; }
        }

        private const float CAMERA_RUNNING_MULTIPLIER = 2.0f;

        public float CAMERA_RUNNING_MULTIPLIER1
        {
            get { return CAMERA_RUNNING_MULTIPLIER; }
        }

        private const float CAMERA_RUNNING_JUMP_MULTIPLIER = 1.5f;

        public float CAMERA_RUNNING_JUMP_MULTIPLIER1
        {
            get { return CAMERA_RUNNING_JUMP_MULTIPLIER; }
        }

        private const float CAMERA_BOUNDS_PADDING = 30.0f;

        public float CAMERA_BOUNDS_PADDING1
        {
            get { return CAMERA_BOUNDS_PADDING; }
        }

        private const float CAMERA_BOUNDS_MIN_X = -FLOOR_PLANE_SIZE / 2.0f + CAMERA_BOUNDS_PADDING;

        public float CAMERA_BOUNDS_MIN_X1
        {
            get { return CAMERA_BOUNDS_MIN_X; }
        }

        private const float CAMERA_BOUNDS_MAX_X = FLOOR_PLANE_SIZE / 2.0f - CAMERA_BOUNDS_PADDING;

        public float CAMERA_BOUNDS_MAX_X1
        {
            get { return CAMERA_BOUNDS_MAX_X; }
        }

        private const float CAMERA_BOUNDS_MIN_Y = 0.0f;

        public float CAMERA_BOUNDS_MIN_Y1
        {
            get { return CAMERA_BOUNDS_MIN_Y; }
        }

        private const float CAMERA_BOUNDS_MAX_Y = WALL_HEIGHT;

        public float CAMERA_BOUNDS_MAX_Y1
        {
            get { return CAMERA_BOUNDS_MAX_Y; }
        }

        private const float CAMERA_BOUNDS_MIN_Z = -FLOOR_PLANE_SIZE / 2.0f + CAMERA_BOUNDS_PADDING;

        public float CAMERA_BOUNDS_MIN_Z1
        {
            get { return CAMERA_BOUNDS_MIN_Z; }
        }

        private const float CAMERA_BOUNDS_MAX_Z = FLOOR_PLANE_SIZE / 2.0f - CAMERA_BOUNDS_PADDING;

        public float CAMERA_BOUNDS_MAX_Z1
        {
            get { return CAMERA_BOUNDS_MAX_Z; }
        }

    }
}

