using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace ShootingGame.Particle
{
    class Explosion
    {
         /// <summary>
        /// This class demonstrates how to combine several different particle systems
        /// to build up a more sophisticated composite effect. It implements a rocket
        /// projectile, which arcs up into the sky using a ParticleEmitter to leave a
        /// steady stream of trail particles behind it. After a while it explodes,
        /// creating a sudden burst of explosion and smoke particles.
        /// </summary>
            #region Constants

            const float trailParticlesPerSecond = 200;
            const int numExplosionParticles = 30;
            const int numExplosionSmokeParticles = 50;
            const float projectileLifespan = 0f;
            const float sidewaysVelocityRange = 60;
            const float verticalVelocityRange = 40;
            const float gravity = 15;

            #endregion

            #region Fields

            ParticleSystem explosionParticles;
            ParticleEmitter trailEmitter;

            Vector3 position;
            float age;
        
            #endregion


            /// <summary>
            /// Constructs a new projectile.
            /// </summary>
            public Explosion(ParticleSystem explosionParticles,
                              ParticleSystem projectileTrailParticles, Vector3 position)
            {
                this.explosionParticles = explosionParticles;

                // Start at the origin, firing in a random (but roughly upward) direction.
                this.position = position;
                // Use the particle emitter helper to output our trail particles.
                trailEmitter = new ParticleEmitter(projectileTrailParticles,
                                                  trailParticlesPerSecond, position);
            }


            /// <summary>
            /// Updates the projectile.
            /// </summary>
            public bool Update(GameTime gameTime)
            {
                float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

                // Simple projectile physics.
                age += elapsedTime;

                // Update the particle emitter, which will create our particle trail.
                trailEmitter.Update(gameTime, position);

                // If enough time has passed, explode! Note how we pass our velocity
                // in to the AddParticle method: this lets the explosion be influenced
                // by the speed and direction of the projectile which created it.
                if (age > projectileLifespan)
                {
                    for (int i = 0; i < numExplosionParticles; i++)
                    {
                        explosionParticles.AddParticle(position, Vector3.Zero);
                    }
                    return false;
               }

                return true;
            }
        }
    
}
