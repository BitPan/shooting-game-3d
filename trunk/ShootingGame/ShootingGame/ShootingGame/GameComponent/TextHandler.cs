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
                tankCommandText = "1. Wander Around\n2. Come Here\n3. Drop Health Globe ( " + scene.GetOctreeWorld().GetTankHealthGlobe() +
                " Left)\n4. Stop";
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

        public void DrawGameFinishText(SpriteFont font, SpriteBatch spriteBatch, GraphicsDevice device, int playerScore)
        {
            Vector2 fontPosition1 = new Vector2(device.Viewport.Width * 0.4f, device.Viewport.Height * 0.5f);
            Vector2 fontPosition2 = new Vector2(device.Viewport.Width * 0.4f, device.Viewport.Height * 0.55f);
            String messageText = "You are dead now.";
            String scoreText = "Your final score is: " + playerScore;

            spriteBatch.Begin();
            // Find the center of the string
            Vector2 FontOrigin1 = font.MeasureString(scoreText);
                Vector2 FontOrigin2 = font.MeasureString(scoreText) / 2;
            // Draw the string

            spriteBatch.DrawString(font, messageText, fontPosition1, Color.Red,
                0, FontOrigin1, 1f, SpriteEffects.None, 0.5f);

            spriteBatch.DrawString(font, scoreText, fontPosition1, Color.Red,
                0, FontOrigin2, 1f, SpriteEffects.None, 0.5f);
            
            spriteBatch.End();

        }

        public void DrawText(SpriteFont font, SpriteBatch spriteBatch, GraphicsDevice device)
        {
            Vector2 fontPosition1 = new Vector2(device.Viewport.Width * 0.9f, device.Viewport.Height * 0.9f);
            Vector2 fontPosition2 = new Vector2(device.Viewport.Width * 0.08f, device.Viewport.Height * 0.98f);
            Vector2 fontPosition3 = new Vector2(device.Viewport.Width * 0.08f, device.Viewport.Height * 0.8f);
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.Default, RasterizerState.CullCounterClockwise);
            // Find the center of the string
            Vector2 FontOrigin = font.MeasureString(scoreText) / 2;
            // Draw the string

            spriteBatch.DrawString(font, scoreText, fontPosition1, Color.LightGreen,
                0, FontOrigin, 1f, SpriteEffects.None, 0.5f);

            if (!tankCommandText.Equals(""))
                spriteBatch.DrawString(font, tankCommandText, fontPosition3, Color.Yellow,
                0, FontOrigin, 0.8f, SpriteEffects.None, 0.5f);


            spriteBatch.DrawString(font, controlTankText, fontPosition2, Color.Yellow,
                0, FontOrigin, 0.8f, SpriteEffects.None, 0.5f);

            spriteBatch.End();
        }

        

    }
}
