using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ShootingGame
{
    class GameMenu
    {
        public bool isSelected = false;
        public String text = "Welcome to the tank world";
        public Rectangle area;

        public GameMenu(Rectangle area, String text) { this.area = area; this.text = text; }
        
        public  void mouseOver(MouseState mousestate) {
            if (mousestate.X > area.X && mousestate.X < area.X + area.Width && mousestate.Y > area.Y && mousestate.Y < area.Y + area.Height)
            {
                isSelected = true;
            }
            else { isSelected = false; }
        }

        public void Draw(SpriteBatch spritebatch, SpriteFont font) {

            if (isSelected == false) { spritebatch.DrawString(font, text, new Vector2(area.X, area.Y), Color.White); }
            if (isSelected == true) { spritebatch.DrawString(font, text, new Vector2(area.X, area.Y), Color.BlueViolet); }
        }
    }
}
