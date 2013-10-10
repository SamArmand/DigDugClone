using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

namespace WindowsGame1
{
    public class Digdug : Sprite
    {

        const string DIGDUG_ASSETNAME = "digdug_sheet2";
        public const int START_POSITION_X = 504;
        public const int START_POSITION_Y = 336;
        public Vector2 originalPosition;
        public Vector2 lastKnownPosition;
        const float DIGDUG_SPEED = 2f;
        const float MOVE_UP = -1f;
        const float MOVE_DOWN = 1f;
        const float MOVE_LEFT = -1f;
        const float MOVE_RIGHT = 1f;

        KeyboardState currentKeyboardState;
        public Stopwatch stopwatch = new Stopwatch();

        public bool wasUp, wasDown, wasLeft, wasRight;
        Hose hose;

        enum State
        {
            Walking
        }
        State currentState = State.Walking;

        Vector2 direction = Vector2.Zero;
        Vector2 speed = Vector2.Zero;

        public Digdug(Hose hose)
        {
            originalPosition = new Vector2(START_POSITION_X,START_POSITION_Y);
            Visible = true;
            source = new Rectangle(536, 176, 56, 56);
            this.hose = hose;
        }

        public void LoadContent(ContentManager contentManager)
        {
            stopwatch.Start();
            Position = new Vector2(START_POSITION_X, START_POSITION_Y);
            base.LoadContent(contentManager, DIGDUG_ASSETNAME);
        }

        public void Update(GameTime gameTime)
        {
            currentKeyboardState = Keyboard.GetState();

            UpdateMovement(currentKeyboardState);

            base.Update(gameTime, speed, direction);
        }

        public void walk()
        {

            if (stopwatch.ElapsedMilliseconds >= 500)
            {
                source.Y = 174;
                stopwatch.Restart();
            }
            else if (stopwatch.ElapsedMilliseconds >= 250)
                source.Y = 264;

        }

        private void UpdateMovement(KeyboardState currentKeyboardState)
        {
            if (currentState == State.Walking)
            {
                speed = Vector2.Zero;
                direction = Vector2.Zero;

                if (currentKeyboardState.IsKeyDown(Keys.Space))
                {

                    if (wasLeft)
                    {

                        hose.source = new Rectangle(0, 0, 56, 56);
                        hose.Position.Y = Position.Y;
                        hose.Position.X = Position.X - 56;
                        //walk();

                    }

                    if (wasRight)
                    {

                        hose.source = new Rectangle(56, 0, 56, 56);
                        hose.Position.Y = Position.Y;
                        hose.Position.X = Position.X + 56;
                        //walk();

                    }

                    if (wasUp)
                    {

                        hose.source = new Rectangle(112, 0, 56, 56);
                        hose.Position.Y = Position.Y - 56;
                        hose.Position.X = Position.X;
                        //walk();

                    }

                    if (wasDown)
                    {

                        hose.source = new Rectangle(168, 0, 56, 56);
                        hose.Position.Y = Position.Y + 56;
                        hose.Position.X = Position.X;
                        //walk();

                    }

                }

                else if (currentKeyboardState.IsKeyDown(Keys.Left) == true)
                {

                    if (Position.Y % 56 != 0)
                    {

                        if (wasUp)
                        {

                            wasLeft = false;
                            wasRight = false;
                            wasUp = true;
                            wasDown = false;
                            speed.Y = DIGDUG_SPEED;
                            direction.Y = MOVE_UP;
                            source = new Rectangle(624, 176, 56, 56);
                            walk();

                        }

                        if (wasDown)
                        {

                            wasLeft = false;
                            wasRight = false;
                            wasUp = false;
                            wasDown = true;
                            speed.Y = DIGDUG_SPEED;
                            direction.Y = MOVE_DOWN;
                            source = new Rectangle(808, 176, 56, 56);
                            walk();

                        }

                    }

                    else
                    {
                        wasLeft = true;
                        wasRight = false;
                        wasUp = false;
                        wasDown = false;
                        speed.X = DIGDUG_SPEED;
                        direction.X = MOVE_LEFT;
                        source = new Rectangle(536, 176, 56, 56);
                        walk();
                    }
                }
                else if (currentKeyboardState.IsKeyDown(Keys.Right) == true)
                {

                    if (Position.Y % 56 != 0)
                    {

                        if (wasUp)
                        {

                            wasLeft = false;
                            wasRight = false;
                            wasUp = true;
                            wasDown = false;
                            speed.Y = DIGDUG_SPEED;
                            direction.Y = MOVE_UP;
                            source = new Rectangle(624, 176, 56, 56);

                            walk();

                        }

                        if (wasDown)
                        {

                            wasLeft = false;
                            wasRight = false;
                            wasUp = false;
                            wasDown = true;
                            speed.Y = DIGDUG_SPEED;
                            direction.Y = MOVE_DOWN;
                            source = new Rectangle(808, 176, 56, 56);

                            walk();

                        }

                    }

                    else
                    {
                        wasLeft = false;
                        wasRight = true;
                        wasUp = false;
                        wasDown = false;
                        speed.X = DIGDUG_SPEED;
                        direction.X = MOVE_RIGHT;
                        source = new Rectangle(712, 176, 56, 56);

                        walk();

                    }
                }

                else if (currentKeyboardState.IsKeyDown(Keys.Up) == true)
                {

                    if (Position.X % 56 != 0)
                    {

                        if (wasLeft)
                        {

                            wasLeft = true;
                            wasRight = false;
                            wasUp = false;
                            wasDown = false;
                            speed.X = DIGDUG_SPEED;
                            direction.X = MOVE_LEFT;
                            source = new Rectangle(536, 176, 56, 56);

                            walk();

                        }

                        if (wasRight)
                        {

                            wasLeft = false;
                            wasRight = true;
                            wasUp = false;
                            wasDown = false;
                            speed.X = DIGDUG_SPEED;
                            direction.X = MOVE_RIGHT;
                            source = new Rectangle(712, 176, 56, 56);

                            walk();

                        }

                    }

                    else
                    {

                        wasLeft = false;
                        wasRight = false;
                        wasUp = true;
                        wasDown = false;
                        speed.Y = DIGDUG_SPEED;
                        direction.Y = MOVE_UP;
                        source = new Rectangle(624, 176, 56, 56);

                        walk();

                    }
                }
                else if (currentKeyboardState.IsKeyDown(Keys.Down) == true)
                {

                    if (Position.X % 56 != 0)
                    {

                        if (wasLeft)
                        {

                            wasLeft = true;
                            wasRight = false;
                            wasUp = false;
                            wasDown = false;
                            speed.X = DIGDUG_SPEED;
                            direction.X = MOVE_LEFT;
                            source = new Rectangle(536, 176, 56, 56);

                            walk();

                        }

                        if (wasRight)
                        {

                            wasLeft = false;
                            wasRight = true;
                            wasUp = false;
                            wasDown = false;
                            speed.X = DIGDUG_SPEED;
                            direction.X = MOVE_RIGHT;
                            source = new Rectangle(712, 176, 56, 56);

                            walk();

                        }

                    }

                    else
                    {

                        wasLeft = false;
                        wasRight = false;
                        wasUp = false;
                        wasDown = true;
                        speed.Y = DIGDUG_SPEED;
                        direction.Y = MOVE_DOWN;
                        source = new Rectangle(808, 176, 56, 56);

                        walk();

                    }
                }

                


                if (Position.X < 0)
                    Position.X = 0;
                if (Position.X > 56 * 17)
                    Position.X = 56 * 17;
                if (Position.Y < 0)
                    Position.Y = 0;
                if (Position.Y > 672)
                    Position.Y = 672;


            }
        }

    }
}
