using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using ShootingGame.Core;

namespace ShootingGame.GameComponent
{
    class TextHandler
    {
        SpriteFont Font1;
        String scoreText;

        public void UpdateText(SceneManager scene)
        {
            scoreText = "Health: " + scene.GetPlayerHealth + "\nScore:" + scene.GetPlayerScore() +"\n" + scene.GetGameLevel();            
        }


        public void GetText()
        {

        }

        public void DrawText(SpriteFont font, SpriteBatch spriteBatch, GameTime gameTime, Vector2 fontPosition)
        {

            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.Default, RasterizerState.CullCounterClockwise);
            // Find the center of the string
            Vector2 FontOrigin = font.MeasureString(scoreText) / 2;
            // Draw the string
            spriteBatch.DrawString(font, scoreText, fontPosition, Color.LightGreen,
                0, FontOrigin, 1.0f, SpriteEffects.None, 0.5f);

            /*
            if (levelUpTextTimer > 0)
            {
                levelUpTextTimer -= gameTime.ElapsedGameTime.Milliseconds;
                Vector2 textSize = Font1.MeasureString(levelUpText);
                spriteBatch.DrawString(Font1, levelUpText,
                    new Vector2((Window.ClientBounds.Width / 2) - (textSize.X / 2),
                        (Window.ClientBounds.Height / 5) - (textSize.Y / 2)), Color.Goldenrod);
            }
             * 
             * */
            spriteBatch.End();
        }

        

    }
}
