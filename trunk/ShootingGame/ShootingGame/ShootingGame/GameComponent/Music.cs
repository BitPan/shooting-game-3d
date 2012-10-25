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


namespace ShootingGame
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Music : Microsoft.Xna.Framework.GameComponent
    {

        private Song song;
        private SoundEffectInstance se;
        SoundEffect shootingEffect;
        SoundEffectInstance shootingEffectInstance;
        SoundEffectInstance hitSound;
        public Music(Game game)
            : base(game)
        {
            this.se = this.Game.Content.Load<SoundEffect>("music/Bomb").CreateInstance();
            this.song = this.Game.Content.Load<Song>("music/background");
            this.shootingEffect = this.Game.Content.Load<SoundEffect>("music/laser");

            this.shootingEffectInstance = shootingEffect.CreateInstance();
            this.hitSound = this.Game.Content.Load<SoundEffect>("music/hit").CreateInstance();
            // TODO: Construct any child components here
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
    
            // TODO: Add your initialization code here

            base.Initialize();
        }

        public void EffectPlay(){
            this.se.Volume = 1f;
            this.se.Play();
        
        }

        public void hitSoundPlay() {
            this.hitSound.Volume = 0.5f;
            this.hitSound.Play();
        
        }
        public void EffectStopPlay() {
            this.se.Stop();
        }

        public void PlayShootingEffect() { 
        this.shootingEffectInstance.Volume=0.5f;
        this.shootingEffectInstance.Play();
        
        }
        public void BackgroundPause() {
            MediaPlayer.Pause();        
        }


        public void BackGroundResume() {

            MediaPlayer.Resume();
        }
        public void BackGroundPlay() {
            MediaPlayer.Volume = 0.1f;
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(this.song);
        
        
        }
        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here

            base.Update(gameTime);
        }
    }
}
