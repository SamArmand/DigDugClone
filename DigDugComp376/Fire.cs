using Microsoft.Xna.Framework;

namespace DigDugComp376
{
    sealed class Fire : Sprite
    {
		internal Fire() : base(MainGame.FireTexture) => Source = new Rectangle(0, 0, 56, 56);

		internal override void Update() => throw new System.NotImplementedException();
	}
}
