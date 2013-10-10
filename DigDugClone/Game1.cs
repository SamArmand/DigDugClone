using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Diagnostics;

namespace WindowsGame1
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont CourierNew;
        Texture2D level1tile;
        Texture2D level2tile;
        Texture2D level3tile;
        Texture2D level4tile;
        public static Stopwatch stopwatch = new Stopwatch();

        KeyboardState currentKeyboardState;
        int pauseCounter;

        Monster[] monsters1 = new Monster[4];
        Monster[] monsters2 = new Monster[6];
        Monster[] monsters;

        Rock[] rocks = new Rock[3];

        public static int score;
        int levelnum;
        int lives;
        public static int deadMonsters;
        bool win;
        bool pause;

        Hose hose;
        public static Digdug digdug;

        public static int[,] level;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = 960;
            graphics.PreferredBackBufferHeight = 546;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            pause = false;
            win = false;
            levelReset();
            hose = new Hose();
            digdug = new Digdug(hose);
            rocks[0] = new Rock(5, 5);
            rocks[1] = new Rock(9,10);
            rocks[2] = new Rock(14,3);

            currentKeyboardState = Keyboard.GetState();
            pauseCounter = 0;

            monsters1[0] = new Monster(hose, 1, 3, "pooka", rocks,2);
            monsters1[1] = new Monster(hose, 5, 9, "fygar", rocks,2);
            monsters1[2] = new Monster(hose, 12, 2, "pooka", rocks,2);
            monsters1[3] = new Monster(hose, 11, 10, "pooka", rocks,2);

            monsters2[0] = new Monster(hose, 1, 3, "pooka", rocks,4);
            monsters2[1] = new Monster(hose, 5, 9, "fygar", rocks,4);
            monsters2[2] = new Monster(hose, 12, 2, "pooka", rocks,4);
            monsters2[3] = new Monster(hose, 11, 10, "pooka", rocks,4);
            monsters2[4] = new Monster(hose, 1, 4, "fygar", rocks,4);
            monsters2[5] = new Monster(hose, 13, 2, "fygar", rocks,4);

            deadMonsters = 0;
            score = 0;
            levelnum = 1;
            lives = 2;
            stopwatch.Start();
            base.Initialize();
            
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            CourierNew = this.Content.Load<SpriteFont>("CourierNew");
            level1tile = this.Content.Load<Texture2D>("level1tile");
            level2tile = this.Content.Load<Texture2D>("level2tile");
            level3tile = this.Content.Load<Texture2D>("level3tile");
            level4tile = this.Content.Load<Texture2D>("level4tile");

            for (int i = 0; i < rocks.Length; i++)
            {
                rocks[i].LoadContent(this.Content);
            }

            for (int i = 0; i < monsters1.Length; i++)
            {
                monsters1[i].LoadContent(this.Content);
            }

            for (int i = 0; i < monsters2.Length; i++)
            {
                monsters2[i].LoadContent(this.Content);
            }

            //level2 monsters

            hose.LoadContent(this.Content);
            digdug.LoadContent(this.Content);

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {

            currentKeyboardState = Keyboard.GetState();

            if (pause && currentKeyboardState.IsKeyDown(Keys.Enter))
            {

   
                if (lives == 0 || win)
                {
                    Initialize();

                }

                pause = false;

            }

            if ((lives > 0) && (!pause))
            {

                if (levelnum == 1)
                    monsters = monsters1;
                else
                    monsters = monsters2;

                // Allows the game to exit
                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                    this.Exit();

                // TODO: Add your update logic here

                digdug.Update(gameTime);
                hose.Update(gameTime);
                for (int i = 0; i < rocks.Length; i++)
                {
                    rocks[i].Update(gameTime);
                }

               
                    for (int i = 0; i < monsters.Length; i++)
                    {

                        if ((digdug.Collides(monsters[i]) || digdug.Collides(monsters[i].fire)) && (monsters[i].currentState == Monster.State.Walking || monsters[i].currentState == Monster.State.Fire))
                            Die();
                        monsters[i].Update(gameTime);
                       
                    }


                //level2 monsters

                int bound0 = level.GetUpperBound(0);
                int bound1 = level.GetUpperBound(1);

                for (int i = 0; i <= bound0; i++)
                {

                    for (int j = 0; j <= bound1; j++)
                    {
                        if (digdug.Position.X == i * 56 && digdug.Position.Y == j * 56)
                        {
                            level[i, j] = 0;
                            digdug.lastKnownPosition.X = digdug.Position.X;
                            digdug.lastKnownPosition.Y = digdug.Position.Y;
                        }
                    }

                }

                for (int i = 0; i < rocks.Length; i++)
                {
                    if (digdug.Collides(rocks[i]) && (rocks[i].currentState == Rock.State.Fixed || rocks[i].currentState == Rock.State.Rumble))
                    {

                        if (digdug.wasLeft)
                            digdug.Position.X += 2;
                        if (digdug.wasRight)
                            digdug.Position.X -= 2;
                        if (digdug.wasUp)
                            digdug.Position.Y += 2;
                        if (digdug.wasDown)
                            digdug.Position.Y -= 2;

                    }

                    if (digdug.Collides(rocks[i]) && rocks[i].currentState == Rock.State.Falling)
                        Die();
                }


                    if (deadMonsters == monsters.Length)
                    {

                             WinLevel();

                    }

            }

            base.Update(gameTime);
            
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here
            spriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.PointClamp, null, null, null, Matrix.CreateScale(0.75f));

            hose.Draw(this.spriteBatch);

            int bound0 = level.GetUpperBound(0);
            int bound1 = level.GetUpperBound(1);

            for (int i = 0; i <= bound0; i++)
             {

                 for (int j = 0; j <= bound1; j++)
                {
                    if (level[i,j] == 1 )
                        spriteBatch.Draw(level1tile, new Vector2(i * 56, j * 56), Color.White);
                    else if (level[i, j] == 2)
                        spriteBatch.Draw(level2tile, new Vector2(i * 56, j * 56), Color.White);
                    else if (level[i, j] == 3)
                        spriteBatch.Draw(level3tile, new Vector2(i * 56, j * 56), Color.White);
                    else if (level[i, j] == 4)
                        spriteBatch.Draw(level4tile, new Vector2(i * 56, j * 56), Color.White);
                }

            }

            digdug.Draw(this.spriteBatch);

            if (deadMonsters < monsters.Length)
            {
                for (int i = 0; i < monsters.Length; i++)
                {
                    monsters[i].Draw(this.spriteBatch);
                    monsters[i].fire.Draw(this.spriteBatch);
                }
            }

            //level2 monsters

            for (int i = 0; i < rocks.Length; i++)
            {
                rocks[i].Draw(this.spriteBatch);
            }

            spriteBatch.DrawString(CourierNew, "Score: " + score, new Vector2(1064, 0), Color.Green);
            spriteBatch.DrawString(CourierNew, "Level: " + levelnum, new Vector2(1064, 112), Color.Green);
            
            if (win)
               spriteBatch.DrawString(CourierNew, "YOU WIN!!!", new Vector2(1064, 224), Color.Green);
            else if (lives == 0)
                spriteBatch.DrawString(CourierNew, "GAME OVER", new Vector2(1064, 224), Color.Green);
            else if (lives > 0)
                spriteBatch.DrawString(CourierNew, "Lives: " + lives, new Vector2(1064, 224), Color.Green);

            if (pause)
            {
                spriteBatch.DrawString(CourierNew, "HIT ENTER", new Vector2(1064, 336), Color.Green);
                spriteBatch.DrawString(CourierNew, "TO RESUME", new Vector2(1064, 400), Color.Green);
            }

            spriteBatch.End();

            base.Draw(gameTime);

        }

        public void Die()
        {

            lives--;
            digdug.Position.X = Digdug.START_POSITION_X;
            digdug.Position.Y = Digdug.START_POSITION_Y;


            if (lives == 0)
            {
                pause = true;
            }

            for (int i = 0; i < monsters.Length; i++)
            {

                monsters[i].Position = monsters[i].originalPosition;

            }

            pause = true;

        }

        public void levelReset()
        {

            level = new int[,] {
        
{0,1,1,1,2,2,2,3,3,3,4,4,4,4},
{0,1,0,0,0,0,2,3,3,3,4,4,4,4},
{0,1,1,1,2,2,2,3,3,3,4,4,4,4},
{0,1,1,1,2,2,2,3,3,0,4,4,4,4},
{0,1,1,1,2,2,2,3,3,0,4,4,4,4},
{0,1,1,1,2,2,2,3,3,0,4,4,4,4},
{0,1,1,1,2,2,2,3,3,0,4,4,4,4},
{0,1,1,1,2,2,2,3,3,3,4,4,4,4},
{0,1,1,1,2,2,2,3,3,3,4,4,4,4},
{0,1,1,1,2,2,0,3,3,3,4,4,4,4},
{0,1,0,1,2,2,2,3,3,3,4,4,4,4},
{0,1,0,1,2,2,2,3,0,0,0,0,4,4},
{0,1,0,1,2,2,2,3,3,3,4,4,4,4},
{0,1,0,1,2,2,2,3,3,3,4,4,4,4},
{0,1,1,1,2,2,2,3,3,3,4,4,4,4},
{0,1,1,1,2,2,2,3,3,3,4,4,4,4},
{0,1,1,1,2,2,2,3,3,3,4,4,4,4},
{0,1,1,1,2,2,2,3,3,3,4,4,4,4},   
 
};

        }

        public void WinLevel()
        {

            pause = true;

            digdug.Position = digdug.originalPosition;

            for (int i = 0; i < rocks.Length; i++)
            {

                rocks[i].Position = rocks[i].originalPosition;
                rocks[i].X = rocks[i].originalX;
                rocks[i].Y = rocks[i].originalY;
                rocks[i].Visible = true;
                rocks[i].currentState = Rock.State.Fixed;

            }

            deadMonsters = 0;
            stopwatch.Restart();
            levelReset();

            if (levelnum == 1)
            {
                levelnum++;
            }

            else
            {
                win = true;
                pause = true;
            }
        }

    }
}
