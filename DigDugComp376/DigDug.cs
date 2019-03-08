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

        const byte Speed = 2;

		readonly Stopwatch _stopwatch = new Stopwatch();

        readonly Rectangle _rectangleUp = new Rectangle(624, 176, 56, 56),
                           _rectangleDown = new Rectangle(808, 176, 56, 56),
                           _rectangleLeft = new Rectangle(536, 176, 56, 56),
                           _rectangleRight = new Rectangle(712, 176, 56, 56),
                           _hoseRectangleUp = new Rectangle(112, 0, 56, 56),
                           _hoseRectangleDown = new Rectangle(168, 0, 56, 56),
                           _hoseRectangleLeft = new Rectangle(0, 0, 56, 56),
                           _hoseRectangleRight = new Rectangle(56, 0, 56, 56);

        const short StartPositionX = 504,
                    StartPositionY = 336;

        bool _isUp,
             _isDown,
             _isLeft,
             _isRight;

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
                if (_isLeft)
                {
                    Hose.Source = _hoseRectangleLeft;
                    Hose.Position.Y = y;
                    Hose.Position.X = x - 56;
                }

                else if (_isRight)
                {
                    Hose.Source = _hoseRectangleRight;
                    Hose.Position.Y = y;
                    Hose.Position.X = x + 56;
                }

                else if (_isUp)
                {
                    Hose.Source = _hoseRectangleUp;
                    Hose.Position.Y = y - 56;
                    Hose.Position.X = x;
                }

                else if (_isDown)
                {
                    Hose.Source = _hoseRectangleDown;
                    Hose.Position.Y = y + 56;
                    Hose.Position.X = x;
                }
            }

            else if (keyboardState.IsKeyDown(Keys.Left))
            {
                if (Math.Abs(y % 56) > 0)
                {
                    if (_isUp) Position.Y -= Speed;

                    else if (_isDown) Position.Y += Speed;
                }

                else
                {
                    _isLeft = true;
                    _isRight = false;
                    _isUp = false;
                    _isDown = false;
                    Position.X -= Speed;
                    Source = _rectangleLeft;
                }

                Walk();
            }

            else if (keyboardState.IsKeyDown(Keys.Right))
            {
                if (Math.Abs(y % 56) > 0)
                {
                    if (_isUp) Position.Y -= Speed;

                    else if (_isDown) Position.Y += Speed;
                }

                else
                {
                    _isLeft = false;
                    _isRight = true;
                    _isUp = false;
                    _isDown = false;
                    Position.X += Speed;
                    Source = _rectangleRight;
                }

                Walk();
            }

            else if (keyboardState.IsKeyDown(Keys.Up))
            {
                if (Math.Abs(x % 56) > 0)
                {
                    if (_isLeft) Position.X -= Speed;

                    else if (_isRight) Position.X += Speed;
                }

                else
                {
                    _isLeft = false;
                    _isRight = false;
                    _isUp = true;
                    _isDown = false;
                    Position.Y -= Speed;
                    Source = _rectangleUp;
                }

                Walk();
            }

            else if (keyboardState.IsKeyDown(Keys.Down))
            {
                if (Math.Abs(x % 56) > 0)
                {
                    if (_isLeft) Position.X -= Speed;

                    else if (_isRight) Position.X += Speed;
                }

                else
                {
                    _isLeft = false;
                    _isRight = false;
                    _isUp = false;
                    _isDown = true;
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
            if (_isLeft) Position.X += Speed;
            else if (_isRight) Position.X -= Speed;
            else if (_isUp) Position.Y += Speed;
            else if (_isDown) Position.Y -= Speed;
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
