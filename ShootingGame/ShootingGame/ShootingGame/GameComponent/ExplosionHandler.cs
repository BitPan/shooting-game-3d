using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShootingGame.Particle;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace ShootingGame.GameComponent
{
    public class ExplosionHandler : Microsoft.Xna.Framework.GameComponent
    {

        ParticleSystem explosionParticles;
        ParticleSystem projectTrialParticles;
        List<Explosion> explosions = new List<Explosion>();

        /// <summary>
        /// Constructs a new projectile.
        /// </summary>
        public ExplosionHandler(Game game)
            : base(game)
        {
            explosionParticles = new ExplosionParticleSystem(game, game.Content);
            projectTrialParticles = new ProjectileTrailParticleSystem(game, game.Content);
            game.Components.Add(explosionParticles);
            game.Components.Add(projectTrialParticles);
        }

        public void CreateExplosion(Vector3 position)
        {
            explosions.Add(new Explosion(explosionParticles,
                                               projectTrialParticles, position));
           
        }


        /// <summary>
        /// Updates the explosion.
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            int i = 0;

            while (i < explosions.Count)
            {
                if (!explosions[i].Update(gameTime))
                {
                    // Remove projectiles at the end of their life.
                    explosions.RemoveAt(i);
                }
                else
                {
                    // Advance to the next projectile.
                    i++;
                }
            }
        }

        public void Draw(GameTime gameTime, FirstPersonCamera camera)
        {
            explosionParticles.SetCamera(camera.ViewMatrix, camera.ProjectionMatrix);
            projectTrialParticles.SetCamera(camera.ViewMatrix, camera.ProjectionMatrix);
        }
    }

}
