using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System.Diagnostics;

namespace WindowsGame1
{
    public class Rock : Sprite
    {

        const string ROCK_ASSETNAME = "rock";
        const float ROCK_SPEED = 1f;
        Vector2 origin;
        int rumbleInt = 0;
        Stopwatch stopwatch;

        Random r = new Random();
        int nextValue;

        public int X, Y, originalX, originalY;
        public Vector2 originalPosition;
        public enum State
        {
            Fixed,
            Rumble,
            Falling,
            Gone
        }
        
        public State currentState = State.Fixed;

        Vector2 direction = Vector2.Zero;
        Vector2 speed = Vector2.Zero;

        public Rock(int X, int Y)
        {
            nextValue = r.Next(2000, 4000);
            stopwatch = new Stopwatch();
            Visible = true;
            this.X = X;
            this.Y = Y;
            originalX = X;
            originalY = Y;
            originalPosition = new Vector2(X * 56, Y * 56);
            source = new Rectangle(0, 0, 56, 56);
            Position = new Vector2(X*56,Y*56);
            origin = new Vector2(X * 56, Y * 56);
        }

        public void LoadContent(ContentManager contentManager)
        {
            base.LoadContent(contentManager, ROCK_ASSETNAME);
        }

        public void Update(GameTime gameTime)
        {

            if (Visible)
            {

                if (currentState == State.Fixed)
                    direction.Y = 0f;

                if (currentState == State.Fixed && Game1.level[X, Y + 1] == 0) {
                    currentState = State.Rumble;
                    stopwatch.Start();
                }

                else if (currentState == State.Rumble && stopwatch.ElapsedMilliseconds < nextValue)
                {

                    if (rumbleInt == 0)
                    {
                        Position.X = origin.X - 2;
                        rumbleInt++;
                    }

                    else if (rumbleInt == 1)
                    {

                        Position.X = origin.X + 2;
                        rumbleInt--;
                    }

                }

                else if (currentState == State.Rumble && stopwatch.ElapsedMilliseconds >= nextValue)
                {

                    Position.X = origin.X;
                    currentState = State.Falling;
                    stopwatch.Stop();
                    stopwatch.Reset();

                }

                else if (currentState == State.Falling && Game1.level[X, Y + 1] == 0)
                {

                    Position.Y += 4;

                    if (Position.Y % 56 == 0)
                        Y++;

                }

                else if (currentState == State.Falling && Game1.level[X, Y + 1] != 0)
                {

                    currentState = State.Gone;
                    stopwatch.Start();

                }

                else if (currentState == State.Gone && stopwatch.ElapsedMilliseconds >= 1000)
                {

                    Visible = false;
                    stopwatch.Stop();

                }

                 base.Update(gameTime, speed, direction);

            }
        }

    }
}
