using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DigDugComp376
{
	/// <summary>
	/// A abstract class representing game sprites.
	/// </summary>
	abstract class Sprite
    {
		/// <summary>
		/// The current position of the sprite.
		/// </summary>
		internal Vector2 Position = new Vector2(0, 0);

		/// <summary>
		/// The rectangle area of the sprite sheet to display.
		/// </summary>
        internal Rectangle Source;

		/// <summary>
		/// Determines whether the sprite is visible.
		/// </summary>
		internal bool IsVisible;

		/// <summary>
		/// Determines whether the sprite is flipped.
		/// </summary>
		internal bool IsFlipped;

		/// <summary>
		/// A random generator.
		/// </summary>
		protected readonly Random Random = new Random();

		/// <summary>
		/// The texture object used when drawing the sprite.
		/// </summary>
		readonly Texture2D _spriteTexture;

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

			return IsVisible && sprite.IsVisible && x + 56 > x1 && x - 56 < x1 && y + 56 > y1 && y - 56 < y1;
		}

		/// <summary>
		/// Draw the sprite to the screen.
		/// </summary>
		internal void Draw()
		{
			if (IsVisible) MainGame.SpriteBatch.Draw(_spriteTexture, Position, Source, Color.White, 0.0f, Vector2.Zero, 1.0f, IsFlipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
		}

		internal abstract void Update();
    }
}
