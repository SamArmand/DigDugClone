using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace WindowsGame1
{
    public class Fire : Sprite
    {

        const string FIRE_ASSETNAME = "fire";
        const int START_POSITION_X = 0;
        const int START_POSITION_Y = 0;

        public Fire()
        {

            source = new Rectangle(0, 0, 56, 56);

        }

        public void LoadContent(ContentManager contentManager)
        {
            Position = new Vector2(START_POSITION_X, START_POSITION_Y);
            base.LoadContent(contentManager, FIRE_ASSETNAME);
        }

        public void Update(GameTime gameTime)
        {
            

        }

    }
}
