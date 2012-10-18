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


namespace ShootingGame.GameComponent
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class LoadingScene : Microsoft.Xna.Framework.GameComponent
    {
        Game game;
        List<GameMenu> gameMenuList;
        MouseState mousetate;
        MouseState prevmousestate;

        public LoadingScene(Game game)
            : base(game)
        {
            this.game = game;
            gameMenuList = new List<GameMenu>();
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here
            gameMenuList.Add(new GameMenu(new Rectangle(((Game).Window.ClientBounds.Width / 2) - 25, ((Game).Window.ClientBounds.Height / 2) - 100, 100, 14), "Play"));
            gameMenuList.Add(new GameMenu(new Rectangle(((Game).Window.ClientBounds.Width / 2) - 25, ((Game).Window.ClientBounds.Height / 2) - 50, 100, 14), "Exit"));
            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here
            mousetate = Mouse.GetState();

            if (gameMenuList.Count > 0)
            {
                foreach (GameMenu m in gameMenuList)
                {
                    m.mouseOver(mousetate);
                    if (m.isSelected == true &&
                        mousetate.LeftButton == ButtonState.Pressed &&
                        prevmousestate.LeftButton == ButtonState.Released)
                    {
                        if (m.text == "Play")
                            ((Game1)Game).InitializeGameComponents();
                        else if (m.text == "Exit")
                            ((Game1)Game).Exit();
                    }
                }
            }
            base.Update(gameTime);
        }
    }
}
