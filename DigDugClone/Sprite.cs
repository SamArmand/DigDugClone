using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Security.Cryptography;

namespace WindowsGame1
{
    public class Sprite
    {

        //The current position of the Sprite
        public Vector2 Position = new Vector2(0, 0);
        //Asset
        public string assetName;
        //The amount to increase/decrease the size of the original sprite. 
        private float mScale = 1.0f;

        //The texture object used when drawing the sprite
        private Texture2D spriteTexture;

        public Rectangle source;
        public bool Visible = false, flip = false;

        static RandomNumberGenerator _rand = RandomNumberGenerator.Create();

        public static int RandomNext(int min, int max)
        {
            if (min > max) throw new ArgumentOutOfRangeException("min");

            byte[] bytes = new byte[4];

            _rand.GetBytes(bytes);

            uint next = BitConverter.ToUInt32(bytes, 0);

            int range = max - min;

            return (int)((next % range) + min);
        }

        //When the scale is modified throught he property, the Size of the 
        //sprite is recalculated with the new scale applied.
        public float Scale
        {
            get { return mScale; }
            set
            {
                mScale = value;
            }
        }

        public bool Collides(Sprite otherSprite)
        {
            // check if two sprites intersect
            return (this.Position.X + 56 > otherSprite.Position.X &&
                    this.Position.X < otherSprite.Position.X + 56 &&
                    this.Position.Y + 56 > otherSprite.Position.Y &&
                    this.Position.Y < otherSprite.Position.Y + 56 &&
                    Visible && otherSprite.Visible);
        }


        //Load Asset
        public void LoadContent(ContentManager theContentManager, string assetName)
        {
            spriteTexture = theContentManager.Load<Texture2D>(assetName);
            this.assetName = assetName;
        }

        //Update the Sprite and change it's position based on the passed in speed, direction and elapsed time.
        public void Update(GameTime theGameTime, Vector2 theSpeed, Vector2 theDirection)
        {

            Position += theDirection * theSpeed;

        }

        //Draw the sprite to the screen
        public void Draw(SpriteBatch theSpriteBatch)
        {
            
            if (Visible && (flip == false))
                theSpriteBatch.Draw(spriteTexture, Position, source,
                    Color.White, 0.0f, Vector2.Zero, Scale, SpriteEffects.None, 0);
            if (Visible && flip)
               theSpriteBatch.Draw(spriteTexture, Position, source,
                    Color.White, 0.0f, Vector2.Zero, Scale, SpriteEffects.FlipHorizontally, 0);

        }

    }
}
