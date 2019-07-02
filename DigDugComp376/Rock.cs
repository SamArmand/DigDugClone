using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace DigDugComp376
{

    sealed class Rock : Sprite
    {
        internal enum State
        {
            Stationary,
            Rumbling,
            Falling,
            Gone
        }

		internal State CurrentState = State.Stationary;

		readonly Stopwatch _stopwatch = new Stopwatch();

        readonly ushort _nextValue;

		readonly byte _x,
			 		  _y0;

        readonly Vector2 _originalPosition;

        bool _isRumbling;

		byte _y;

        internal Rock(byte x, byte y) : base(MainGame.RockTexture)
        {
            _nextValue = (ushort)Random.Next(2000, 4000);
            IsVisible = true;

			_x = x;
			_y = y;
			_y0 = y;

            _originalPosition = new Vector2(x, y) * 56;
            Source = new Rectangle(0, 0, 56, 56);
            Position = _originalPosition;
        }

		internal override void Update()
        {
            if (!IsVisible) return;

            switch (CurrentState)
            {
                case State.Stationary when MainGame.Level[_x, _y + 1] == 0:
                    CurrentState = State.Rumbling;
                    _isRumbling = false;
                    ++Position.X;
                    _stopwatch.Start();
                    break;
                case State.Rumbling when _stopwatch.ElapsedMilliseconds < _nextValue:
                    switch (_isRumbling)
                    {
                        case false:
                            Position.X -= 2;
                            _isRumbling = true;
                            break;
                        case true:
                            Position.X += 2;
                            _isRumbling = false;
                            break;
                    }
                    break;
                case State.Rumbling when _stopwatch.ElapsedMilliseconds >= _nextValue:
                    Position.X = _originalPosition.X;
                    CurrentState = State.Falling;
                    _stopwatch.Stop();
                    _stopwatch.Reset();
                    break;
                case State.Falling when MainGame.Level[_x, _y + 1] == 0:
                    if (!(Math.Abs((Position.Y += 4) % 56) > 0)) ++_y;
                    break;
                case State.Falling:
                    CurrentState = State.Gone;
                    _stopwatch.Start();
                    break;
                case State.Gone when _stopwatch.ElapsedMilliseconds >= 1000:
                    IsVisible = false;
                    _stopwatch.Stop();
                    break;
            }
        }

		internal void Reset()
		{
			Position = _originalPosition;
			IsVisible = true;
			CurrentState = State.Stationary;
			_y = _y0;
		}
    }
}
