using Microsoft.Xna.Framework.Input;

namespace DigDugComp376
{
	sealed class Hose : Sprite
    {
		internal Hose() : base(Game1.HoseTexture)
		{
		}

		internal override void Update() => IsVisible = Keyboard.GetState().IsKeyDown(Keys.Space);
	}
}
