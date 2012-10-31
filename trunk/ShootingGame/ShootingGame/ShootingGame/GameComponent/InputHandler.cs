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
        KeyboardState previousKeySate;

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
            
            KeyboardState kState = Keyboard.GetState();

            if (previousKeySate.IsKeyDown(Keys.C) && kState.IsKeyUp(Keys.C))
            {
                if (!scene.GetOctreeWorld().IsControlTankEnabled())
                    scene.GetOctreeWorld().EnableControlTank();
                else
                    scene.GetOctreeWorld().DisableControlTank();
            }

            if (previousKeySate.IsKeyDown(Keys.D1) && kState.IsKeyUp(Keys.D1))
            {
                if (scene.GetOctreeWorld().IsControlTankEnabled())
                    scene.GetOctreeWorld().GetTank().ActivateWanderMode();
                scene.GetOctreeWorld().DisableControlTank();
            }

            if (previousKeySate.IsKeyDown(Keys.D2) && kState.IsKeyUp(Keys.D2))
            {
                if (scene.GetOctreeWorld().IsControlTankEnabled())
                    scene.GetOctreeWorld().GetTank().ActivateFollowMode();
                scene.GetOctreeWorld().DisableControlTank();
            }

            if (previousKeySate.IsKeyDown(Keys.D3) && kState.IsKeyUp(Keys.D3))
            {
                if (scene.GetOctreeWorld().IsControlTankEnabled())
                    scene.GetOctreeWorld().UseHealthGlobe();
                scene.GetOctreeWorld().DisableControlTank();
            }

            if (previousKeySate.IsKeyDown(Keys.D4) && kState.IsKeyUp(Keys.D4))
            {
                if (scene.GetOctreeWorld().IsControlTankEnabled())
                    scene.GetOctreeWorld().GetTank().DeactiveActionMode();
                scene.GetOctreeWorld().DisableControlTank();
            }

            if (previousKeySate.IsKeyDown(Keys.D5) && kState.IsKeyUp(Keys.D5))
            {
                scene.DeductPlayerHealth(10);
            }
            
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

            previousKeySate = kState;

        }
    }
}


