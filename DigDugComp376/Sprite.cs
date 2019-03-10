using System;
using System.Security.Cryptography;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DigDugComp376
{
	class Sprite
    {
        //The current position of the Sprite
        internal Vector2 Position = new Vector2(0, 0);

		//The texture object used when drawing the sprite
		readonly Texture2D _spriteTexture;

        internal Rectangle Source;

		internal bool Visible,
					  Flip;

		protected readonly Random Random = new Random();

		internal Sprite(Texture2D spriteTexture) => _spriteTexture = spriteTexture;

		/// <summary>
		/// Checks if the sprite collides with another sprite.
		/// </summary>
		/// <param name="sprite">The sprite against which to check collision.</param>
		/// <returns>Whether or not the sprite collides with the other sprite.</returns>
		internal bool Collides(Sprite sprite)
		{
			var (x, y) = Position;
			var (x1, y1) = sprite.Position;

			return Visible && sprite.Visible && x + 56 > x1 && x - 56 < x1 && y + 56 > y1 && y - 56 < y1;
		}

		/// <summary>
		/// Draw the sprite to the screen
		/// </summary>
		internal void Draw()
		{
			if (Visible) Game1.SpriteBatch.Draw(_spriteTexture, Position, Source, Color.White, 0.0f, Vector2.Zero, 1.0f, Flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
		}
    }
}
