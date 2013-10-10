using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System.Diagnostics;
using System.Security.Cryptography;

namespace WindowsGame1
{
    public class Monster : Sprite
    {

        Stopwatch stopwatch;
        Stopwatch fireWatch;
        Stopwatch ghostWatch;
        Stopwatch timeAsGhost;
        Stopwatch walkWatch;
        Stopwatch deadWatch;

        Vector2 direction = Vector2.Zero;
        Vector2 speed = Vector2.Zero;
        Vector2 lastSpeed, lastDirection;
        public Vector2 originalPosition;
        Vector2 digdugLastKnownPos;

        int monster_speed;
        const float MOVE_UP = -1f;
        const float MOVE_DOWN = 1f;
        const float MOVE_LEFT = -1f;
        const float MOVE_RIGHT = 1f;
        int X, Y, multiplier;
        Hose hose;
        int growCount;
        Rock[] rocks;
        public Fire fire;
        Boolean choose = false;
        int choice = 0;

        //int nextValue;

        public enum State
        {
            Walking,
            Ghost,
            Fire,
            Growing,
            Dead
        }

        public State currentState = State.Walking;
        public State lastState;

        public Monster(Hose hose, int X, int Y, String assetName, Rock[] rocks, int monster_speed)
        {
            deadWatch = new Stopwatch();
            walkWatch = new Stopwatch();
            this.monster_speed = monster_speed;
            this.rocks = rocks;
            multiplier = 1;
            Visible = true;
            this.X = X;
            this.Y = Y;
            originalPosition = new Vector2(X*56, Y*56);
            Position = new Vector2(X * 56, Y * 56);
            source = new Rectangle(56, 0, 56, 56);
            this.hose = hose;
            this.assetName = assetName;
            fire = new Fire();

            if (assetName.Equals("fygar"))
                multiplier = 2;

            stopwatch = new Stopwatch();
            fireWatch = new Stopwatch();
            ghostWatch = new Stopwatch();
            timeAsGhost = new Stopwatch();
            fireWatch.Start();
            ghostWatch.Start();
            walkWatch.Start();
            growCount = 0;
        }

        public void LoadContent(ContentManager contentManager)
        {
            fire.LoadContent(contentManager);
            base.LoadContent(contentManager, assetName);
        }
        
        public void Update(GameTime gameTime)
        {

            speed = Vector2.Zero;
            direction = Vector2.Zero;

            if (Visible)
            {

                if (currentState == State.Dead && deadWatch.ElapsedMilliseconds >= 1000)
                {
                    Visible = false;
                    deadWatch.Stop();
                }

                else if (currentState == State.Walking && (fireWatch.ElapsedMilliseconds >= RandomNext(10000,20000)) && assetName.Equals("fygar"))
                {
                    fire.Visible = true;
                    walkWatch.Reset();
                    currentState = State.Fire;
                    if (flip)
                        fire.Position = new Vector2(Position.X + 56, Position.Y);
                    else
                        fire.Position = new Vector2(Position.X - 56, Position.Y);
                    fireWatch.Restart();
                }

                else if (currentState == State.Walking && (ghostWatch.ElapsedMilliseconds >= RandomNext(10000, 20000)) && Game1.stopwatch.ElapsedMilliseconds >= 5000)
                {
                    walkWatch.Reset();
                    source.X = 0;
                    source.Y = 0;
                    currentState = State.Ghost;
                    digdugLastKnownPos.X = Game1.digdug.lastKnownPosition.X;
                    digdugLastKnownPos.Y = Game1.digdug.lastKnownPosition.Y;
                    ghostWatch.Reset();
                    timeAsGhost.Start();

                }

                else if (currentState == State.Ghost && (Position.X % 56 == 0) && (Position.Y % 56 == 0) && Game1.level[(int)Position.X/56, (int)Position.Y/56] == 0 && timeAsGhost.ElapsedMilliseconds >= 1000)
                {

                    timeAsGhost.Reset();
                    source.X = 56;
                    currentState = State.Walking;
                    walkWatch.Start();
                    ghostWatch.Start();

                }

                else if (currentState == State.Fire && fireWatch.ElapsedMilliseconds >= 2000)
                {

                    fire.Visible = false;
                    fireWatch.Restart();
                    walkWatch.Start();
                    currentState = State.Walking;

                }

                else if ((currentState == State.Walking) && Collides(hose))
                {
                    walkWatch.Reset();
                    lastState = currentState;
                    source.Y = 0;
                    currentState = State.Growing;
                    stopwatch.Start();
                }

                else if (currentState == State.Growing && stopwatch.ElapsedMilliseconds >= 1000)
                {
                    if (Collides(hose))
                        growCount++;
                    else if (growCount > 0)
                        growCount--;

                    if (growCount == 0)
                    {
                        currentState = lastState;
                        stopwatch.Stop();
                    }
                    else if (growCount == 4)
                        Die(1);

                    source.X = 56 + (growCount * 56);

                    if (currentState == State.Ghost)
                        source.X = 0;

                    stopwatch.Restart();

                }

                for (int i = 0; i < rocks.Length; i++)
                {

                    if (rocks[i].currentState == Rock.State.Falling && Collides(rocks[i]) && currentState != State.Dead)
                        Die(2);

                }

               

                UpdateMovement();

                base.Update(gameTime, speed, direction);
            
            }
        }

        public void UpdateMovement() 
        {

            if (currentState == State.Ghost)
            {
                if (digdugLastKnownPos.X > Position.X)
                {

                    direction.X = MOVE_RIGHT;
                    speed.X = (int)(monster_speed/2);

                }

                else if (digdugLastKnownPos.X < Position.X)
                {

                    direction.X = MOVE_LEFT;
                    speed.X = (int)(monster_speed / 2);
                    

                }

                if (digdugLastKnownPos.Y > Position.Y)
                {

                    direction.Y = MOVE_DOWN;
                    speed.Y = (int)(monster_speed / 2);

                }

                else if (digdugLastKnownPos.Y < Position.Y)
                {

                    direction.Y = MOVE_UP;
                    speed.Y = (int)(monster_speed / 2);

                }


                if (direction.X == MOVE_RIGHT)
                {
                    flip = true;
                    fire.flip = true;
                }
                if (direction.X == MOVE_LEFT)
                {
                    flip = false;
                    fire.flip = true;
                }
            }

            else if (currentState == State.Walking)
            {

                if (Position.X % 56 != 0 || Position.Y % 56 != 0)
                {

                    speed = lastSpeed;
                    direction = lastDirection;

                }

                if ((Position.X % 56 == 0) && (Position.Y % 56 == 0)) {

                    choose = false;
                    choice = 0;

                

                while (!choose)
                {

                    choice = RandomNext(0, 4);

                    if (choice == 0 && Position.X != 0)
                    {
                        if (Game1.level[(int)(Position.X / 56) - 1, (int)(Position.Y/56)] == 0)
                        {
                            speed.X = monster_speed;
                            direction.X = MOVE_LEFT;
                            
                            lastSpeed.X = monster_speed;
                            lastDirection.X = MOVE_LEFT;
                            lastSpeed.Y = 0;
                            lastDirection.Y = 0;

                            choose = true;
                        }
                    }

                    else if (choice == 1 && Position.X != 17 * 56)
                    {

                        if (Game1.level[(int)(Position.X / 56) + 1, (int)(Position.Y/56)] == 0)
                        {
                            speed.X = monster_speed;
                            direction.X = MOVE_RIGHT;

                            lastSpeed.X = monster_speed;
                            lastDirection.X = MOVE_RIGHT;
                            lastSpeed.Y = 0;
                            lastDirection.Y = 0;

                            choose = true;
                        }

                    }

                    else if (choice == 2 && Position.Y != 0)
                    {

                        if (Game1.level[(int)(Position.X/56), (int)(Position.Y/56) - 1] == 0)
                        {
                            speed.Y = monster_speed;
                            direction.Y = MOVE_UP;

                            lastSpeed.X = 0;
                            lastDirection.X = 0;
                            lastSpeed.Y = monster_speed;
                            lastDirection.Y = MOVE_UP;

                            choose = true;
                        }

                    }

                    else if (choice == 3 && Position.Y != 13*56)
                    {

                        if (Game1.level[(int)(Position.X / 56), (int)(Position.Y / 56) + 1] == 0)
                        {
                            speed.Y = monster_speed;
                            direction.Y = MOVE_DOWN;

                            lastSpeed.X = 0;
                            lastDirection.X = 0;
                            lastSpeed.Y = monster_speed;
                            lastDirection.Y = MOVE_DOWN;

                            choose = true;
                        }

                    }

                    //
                     if (direction.X == MOVE_RIGHT)
                {
                    flip = true;
                    fire.flip = true;
                }
                     if (direction.X == MOVE_LEFT)
                     {
                         flip = false;
                         fire.flip = false;
                     }
                    //

                }

                }

                walk();

            }

        }

        public void walk()
        {

            if (walkWatch.ElapsedMilliseconds >= 500)
            {
                source.Y = 56;
                walkWatch.Restart();
            }
            else if (walkWatch.ElapsedMilliseconds >= 250)
                source.Y = 0;

        }

        public void Die(int method)
        {

            deadWatch.Start();
            Game1.deadMonsters++;
            currentState = State.Dead;
            Game1.score += multiplier * method * (Y+1) * 10;

        }

    }
}