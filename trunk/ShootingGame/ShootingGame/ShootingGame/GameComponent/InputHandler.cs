using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShootingGame.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ShootingGame.GameComponent
{
    public class InputHandler
    {


        private int timeSinceLastShoot;
        private int nextShootTime;

        public InputHandler()
        {
            timeSinceLastShoot = 0;
            nextShootTime = 500;
        }

        public void UpdateWorld(GameTime gameTime, FirstPersonCamera camera, SceneManager scene, Music music)
        {
            timeSinceLastShoot += gameTime.ElapsedGameTime.Milliseconds;

            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                if (timeSinceLastShoot >= nextShootTime)
                {
                    music.PlayShootingEffect();
                    Vector3 direction = camera.ViewDirection;
                    scene.AddPlayerBulletModel(camera.Position, direction);
                    timeSinceLastShoot = 0;
                }
            }

        }
    }
}


