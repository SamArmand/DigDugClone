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

        internal State CurrentState = Stationary;

        const State Stationary = State.Stationary,
                    Rumbling = State.Rumbling,
                    Falling = State.Falling,
                    Gone = State.Gone;

        readonly Stopwatch _stopwatch = new Stopwatch();

        readonly Random _random = new Random();

        readonly int _nextValue;

		readonly byte _x,
			 		  _y0;

        readonly Vector2 _originalPosition;

        bool _rumble;

		byte _y;

        internal Rock(byte x, byte y) : base(Game1.RockTexture)
        {
            _nextValue = _random.Next(2000, 4000);
            Visible = true;

			_x = x;
			_y = y;
			_y0 = y;

            _originalPosition = new Vector2(x, y) * 56;
            Source = new Rectangle(0, 0, 56, 56);
            Position = _originalPosition;
        }

		internal void Update()
        {
            if (!Visible) return;

            switch (CurrentState)
            {
                case Stationary when Game1.Level[_x, _y + 1] == 0:
                    CurrentState = Rumbling;
                    _rumble = false;
                    ++Position.X;
                    _stopwatch.Start();
                    break;
                case Rumbling when _stopwatch.ElapsedMilliseconds < _nextValue:
                    switch (_rumble)
                    {
                        case false:
                            Position.X -= 2;
                            _rumble = true;
                            break;
                        case true:
                            Position.X += 2;
                            _rumble = false;
                            break;
                    }
                    break;
                case Rumbling when _stopwatch.ElapsedMilliseconds >= _nextValue:
                    Position.X = _originalPosition.X;
                    CurrentState = Falling;
                    _stopwatch.Stop();
                    _stopwatch.Reset();
                    break;
                case Falling when Game1.Level[_x, _y + 1] == 0:
                    if (!(Math.Abs((Position.Y += 4) % 56) > 0)) ++_y;
                    break;
                case Falling:
                    CurrentState = Gone;
                    _stopwatch.Start();
                    break;
                case Gone when _stopwatch.ElapsedMilliseconds >= 1000:
                    Visible = false;
                    _stopwatch.Stop();
                    break;
            }
        }

		internal void Reset()
		{
			Position = _originalPosition;
			Visible = true;
			CurrentState = Stationary;
			_y = _y0;
		}
    }
}
