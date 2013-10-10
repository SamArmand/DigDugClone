using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace WindowsGame1
{
    public class Hose : Sprite
    {

        const string HOSE_ASSETNAME = "hose";
        const int START_POSITION_X = 0;
        const int START_POSITION_Y = 0;

        public void LoadContent(ContentManager contentManager)
        {
            Position = new Vector2(START_POSITION_X, START_POSITION_Y);
            base.LoadContent(contentManager, HOSE_ASSETNAME);
        }

        public void Update(GameTime gameTime)
        {
            KeyboardState currentKeyboardState = Keyboard.GetState();

            if (currentKeyboardState.IsKeyDown(Keys.Space))
                Visible = true;
            else
                Visible = false;

            
        }


    }
}
