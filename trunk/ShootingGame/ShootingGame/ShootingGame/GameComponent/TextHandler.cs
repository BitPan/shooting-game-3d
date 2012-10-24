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
        String controlTankText;
        String tankCommandText;


        public TextHandler()
        {
            
        }

        public void UpdateText(SceneManager scene)
        {
            scoreText = "Health: " + scene.GetPlayerHealth + "\nScore:" + scene.GetPlayerScore() +"\n" + scene.GetGameLevel();
            if (scene.GetOctreeWorld().IsControlTankEnabled())
            {
                tankCommandText = "1. Wander Around\n2. Follow Me\n3. Attack Enemy\n4. Stop";
                controlTankText = "Press C Again To Close Menu";
            }
            else
            {
                tankCommandText = "";
                controlTankText = "Press C To Control Tank";
            }

        }


        public void GetText()
        {

        }

        public void DrawText(SpriteFont font, SpriteBatch spriteBatch, GameTime gameTime, GraphicsDevice device)
        {
            Vector2 fontPosition1 = new Vector2(device.Viewport.Width * 0.9f, device.Viewport.Height * 0.9f);
            Vector2 fontPosition2 = new Vector2(device.Viewport.Width * 0.08f, device.Viewport.Height * 0.98f);
            Vector2 fontPosition3 = new Vector2(device.Viewport.Width * 0.08f, device.Viewport.Height * 0.8f);
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.Default, RasterizerState.CullCounterClockwise);
            // Find the center of the string
            Vector2 FontOrigin = font.MeasureString(scoreText) / 2;
            // Draw the string
            if (!tankCommandText.Equals(""))
                spriteBatch.DrawString(font, tankCommandText, fontPosition3, Color.Yellow,
                0, FontOrigin, 0.8f, SpriteEffects.None, 0.5f);


            spriteBatch.DrawString(font, controlTankText, fontPosition2, Color.Yellow,
                0, FontOrigin, 0.8f, SpriteEffects.None, 0.5f);

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
