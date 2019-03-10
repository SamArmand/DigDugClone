using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DigDugComp376
{
	/// <summary>
	/// This is the main type for your game
	/// </summary>
	sealed class Game1 : Game
	{
		internal static byte[,] Level;

		internal static DigDug DigDug;

		internal static SpriteBatch SpriteBatch;

		internal static Texture2D DigDugTexture,
								  FireTexture,
								  HoseTexture,
								  FygarTexture,
								  PookaTexture,
								  RockTexture;

		internal static readonly Stopwatch Stopwatch = new Stopwatch();

		internal static ushort Score;

		internal static byte DeadMonsters;

		const byte PookaMultiplier = 1,
				  FygarMultiplier = 2;
			
		readonly GraphicsDeviceManager _graphicsDeviceManager;

		readonly Vector2 _sidebarVector = new Vector2(1064, 56);

		byte _levelnum,
			_lives;

		bool _win,
			 _pause,
			 _new;

		SpriteFont _courierNew;

		Texture2D[] _levelTiles;

		Monster[] _monsters1,
				  _monsters2,
				  _monsters;

		Rock[] _rocks;

        internal Game1()
		{
			_new = true;
			_pause = true;

			_graphicsDeviceManager = new GraphicsDeviceManager(this)
			{
				PreferredBackBufferWidth = 960,
				PreferredBackBufferHeight = 546
			};

			Content.RootDirectory = "Content";
		}

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content. Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            _win = false;
			ResetLevel();

            DeadMonsters = 0;
            Score = 0;
            _levelnum = 1;
            _lives = 2;

			Stopwatch.Start();

			base.Initialize();
		}

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            _courierNew = Content.Load<SpriteFont>("Font");

			_levelTiles = new[] {
				Content.Load<Texture2D>("level1tile"),
				Content.Load<Texture2D>("level2tile"),
				Content.Load<Texture2D>("level3tile"),
				Content.Load<Texture2D>("level4tile")
			};

			DigDugTexture = Content.Load<Texture2D>("digdug_sheet2");
			FireTexture = Content.Load<Texture2D>("fire");
			HoseTexture = Content.Load<Texture2D>("hose");
			FygarTexture = Content.Load<Texture2D>("fygar");
			PookaTexture = Content.Load<Texture2D>("pooka");
			RockTexture = Content.Load<Texture2D>("rock");

			DigDug = new DigDug();

			_rocks = new []
					{
						new Rock(5, 5),
						new Rock(9, 10),
						new Rock(14, 3)
					};

			_monsters1 = new[]
						{
							new Monster(1, 3, false, PookaMultiplier, 2),
							new Monster(5, 9, true, FygarMultiplier, 2),
							new Monster(12, 2, false, PookaMultiplier, 2),
							new Monster(11, 10, false, PookaMultiplier, 2)
						};

			_monsters2 = new[]
						{
							new Monster(1, 3, false, PookaMultiplier, 4),
							new Monster(5, 9, true, FygarMultiplier, 4),
							new Monster(12, 2, false, PookaMultiplier, 4),
							new Monster(11, 10, false, PookaMultiplier, 4),
							new Monster(1, 4, true, FygarMultiplier, 4),
							new Monster(13, 2, true, FygarMultiplier, 4)
						};

			_monsters = _monsters1;
		}

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
		{
			_graphicsDeviceManager.Dispose();
			Content.Unload();
		}

		/// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
			var keyboardState = Keyboard.GetState();

			// Allows the game to exit
			if (keyboardState.IsKeyDown(Keys.Escape)) Exit();

			else if (_pause || _new)
            {
				if (keyboardState.IsKeyDown(Keys.Enter))
				{
					if (_lives == 0 || _win) Initialize();

					_monsters = _levelnum == 1 ? _monsters1 : _monsters2;

					foreach (var monster in _monsters) monster.Play();

					_pause = false;
					_new = false;
				}
			}

            else if (_lives > 0)
            {
                DigDug.Update();

				foreach (var rock in _rocks) rock.Update();

				foreach (var monster in _monsters)
				{
					var monsterState = monster.CurrentState;

					if ((monsterState == Monster.State.Walking
						|| monsterState == Monster.State.Dragon) && DigDug.Collides(monster)
						|| DigDug.Collides(monster.Fire)) Die();
					else monster.Update();
				}

				//level2 monsters

				foreach (var rock in _rocks)
                {
					var rockState = rock.CurrentState;

					if (DigDug.Collides(rock))
						switch (rockState)
						{
							case Rock.State.Stationary:
							case Rock.State.Rumbling:
								DigDug.GoBack();
								break;
							case Rock.State.Falling:
								Die();
								break;
						}

					foreach (var monster in _monsters.Where(monster => rockState == Rock.State.Falling && monster.Collides(rock) && monster.CurrentState != Monster.State.Dead)) monster.Die(2);
                }

				if (DeadMonsters == _monsters.Length)
				{
					//Win!
					_monsters = new Monster[] { };

					_pause = true;

					DigDug.Position = DigDug.OriginalPosition;
					DigDug.Hose.Visible = false;

					foreach (var rock in _rocks) rock.Reset();

					DeadMonsters = 0;
					Stopwatch.Restart();
					ResetLevel();

					if (_levelnum == 1) ++_levelnum;

					else
					{
						_win = true;
						_pause = true;
					}
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

            SpriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.PointClamp, null, null, null, Matrix.CreateScale(0.75f));

            var bound0 = Level.GetUpperBound(0);
            var bound1 = Level.GetUpperBound(1);

			var (x, y) = DigDug.Position;

			for (var i = 0; i <= bound0; ++i)
				for (var j = 1; j <= bound1; ++j)
				{
					if (Math.Abs(x - i * 56) <= 0 && Math.Abs(y - j * 56) <= 0) Level[i, j] = 0;
					var block = Level[i, j];
					if (block > 0) SpriteBatch.Draw(_levelTiles[block - 1], new Vector2(i, j) * 56, Color.White);
				}

			DigDug.Draw();
			DigDug.Hose.Draw();

			if (DeadMonsters < _monsters.Length)
				foreach (var monster in _monsters)
				{
					monster.Draw();
					monster.Fire.Draw();
				}

			foreach (var rock in _rocks) rock.Draw();

			SpriteBatch.DrawString(_courierNew,
									"Score: " + Score +
									"\n\n\n\nLevel: " + _levelnum + "\n\n\n\n" +
									(_win ? "YOU WIN!!!" : _lives == 0 ? "GAME OVER" : "Lives: " + _lives) +
									(_pause ? "\n\n\n\nHIT ENTER\nTO " + (_new ? "START" : "RESUME") : ""), _sidebarVector, Color.Green);

            SpriteBatch.End();

            base.Draw(gameTime);
        }

		static void ResetLevel() => Level = new byte[,] {
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
														   {0,1,1,1,2,2,2,3,3,3,4,4,4,4}
														};

		void Die()
        {
			_pause = true;

            --_lives;

			DigDug.Reset();

			foreach (var monster in _monsters) monster.Reset();
		}
	}
}
