using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace DigDugComp376
{
    sealed class DigDug : Sprite
    {
        internal readonly Hose Hose = new Hose();

        internal Vector2 OriginalPosition;

		internal enum Direction
		{
			Up,
			Down,
			Left,
			Right
		}

		const byte Speed = 2;

		const short StartPositionX = 504,
					StartPositionY = 336;

		readonly Stopwatch _stopwatch = new Stopwatch();

        readonly Rectangle _rectangleUp = new Rectangle(624, 176, 56, 56),
                           _rectangleDown = new Rectangle(808, 176, 56, 56),
                           _rectangleLeft = new Rectangle(536, 176, 56, 56),
                           _rectangleRight = new Rectangle(712, 176, 56, 56),
                           _hoseRectangleUp = new Rectangle(112, 0, 56, 56),
                           _hoseRectangleDown = new Rectangle(168, 0, 56, 56),
                           _hoseRectangleLeft = new Rectangle(0, 0, 56, 56),
                           _hoseRectangleRight = new Rectangle(56, 0, 56, 56);

		Direction _direction;

		internal DigDug() : base(Game1.DigDugTexture)
        {
            OriginalPosition = new Vector2(StartPositionX,StartPositionY);
            Visible = true;
            Source = _rectangleLeft;
            _stopwatch.Start();
            Position = new Vector2(StartPositionX, StartPositionY);
        }

        internal void Update()
        {
			var keyboardState = Keyboard.GetState();

            var (x, y) = Position;

			if (keyboardState.IsKeyDown(Keys.Space))
            {
				switch (_direction)
				{
					case Direction.Left:
						Hose.Source = _hoseRectangleLeft;
						Hose.Position.Y = y;
						Hose.Position.X = x - 56;
						break;
					case Direction.Right:
						Hose.Source = _hoseRectangleRight;
						Hose.Position.Y = y;
						Hose.Position.X = x + 56;
						break;
					case Direction.Up:
						Hose.Source = _hoseRectangleUp;
						Hose.Position.Y = y - 56;
						Hose.Position.X = x;
						break;
					case Direction.Down:
						Hose.Source = _hoseRectangleDown;
						Hose.Position.Y = y + 56;
						Hose.Position.X = x;
						break;
				}
			}

            else if (keyboardState.IsKeyDown(Keys.Left))
            {
				if (Math.Abs(y % 56) > 0) Position.Y += Speed * (_direction == Direction.Up ? -1 : _direction == Direction.Down ? 1 : 0);

				else
                {
					_direction = Direction.Left;
                    Position.X -= Speed;
                    Source = _rectangleLeft;
                }

                Walk();
            }

            else if (keyboardState.IsKeyDown(Keys.Right))
            {
				if (Math.Abs(y % 56) > 0) Position.Y += Speed * (_direction == Direction.Up ? -1 : _direction == Direction.Down ? 1 : 0);

                else
                {
					_direction = Direction.Right;
                    Position.X += Speed;
                    Source = _rectangleRight;
                }

                Walk();
            }

            else if (keyboardState.IsKeyDown(Keys.Up))
            {
				if (Math.Abs(x % 56) > 0) Position.X += Speed * (_direction == Direction.Left ? -1 : _direction == Direction.Right ? 1 : 0);

				else
				{
					_direction = Direction.Up;
					Position.Y -= Speed;
					Source = _rectangleUp;
				}

				Walk();
            }

            else if (keyboardState.IsKeyDown(Keys.Down))
            {
				if (Math.Abs(x % 56) > 0) Position.X += Speed * (_direction == Direction.Left ? -1 : _direction == Direction.Right ? 1 : 0);

				else
				{
					_direction = Direction.Down;
					Position.Y += Speed;
					Source = _rectangleDown;
				}

				Walk();
            }

            var (x1, y1) = Position;
            if (x1 < 0 || x1 > 952 || y1 < 0 || y1 > 672) GoBack();

            Hose.Update();
        }

        internal void GoBack()
        {
			switch (_direction)
			{
				case Direction.Left:
					Position.X += Speed;
					break;
				case Direction.Right:
					Position.X -= Speed;
					break;
				case Direction.Up:
					Position.Y += Speed;
					break;
				case Direction.Down:
					Position.Y -= Speed;
					break;
			}
		}

        internal void Reset()
        {
            Hose.Visible = false;
            Position.X = StartPositionX;
            Position.Y = StartPositionY;
            Source = _rectangleLeft;
        }

        void Walk()
        {
            var elapsed = _stopwatch.ElapsedMilliseconds;

            if (elapsed >= 500)
            {
                Source.Y = 174;
                _stopwatch.Restart();
            }

            else if (elapsed >= 250) Source.Y = 264;
        }
    }
}
