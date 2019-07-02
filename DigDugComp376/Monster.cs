using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace DigDugComp376
{
	sealed class Monster : Sprite
	{
		internal readonly Fire Fire = new Fire();

		internal enum State
		{
			Walking,
			Ghost,
			Dragon,
			Growing,
			Dead
		}

		internal State CurrentState = State.Walking;

		readonly bool _isFygar;

		readonly byte _monsterSpeed,
					 _y,
					 _multiplier;

		readonly Stopwatch _stopwatch = new Stopwatch(),
						   _fireWatch = new Stopwatch(),
						   _ghostWatch = new Stopwatch(),
						   _timeAsGhost = new Stopwatch(),
						   _walkWatch = new Stopwatch(),
						   _deadWatch = new Stopwatch(),
						   _game1Stopwatch = Game1.Stopwatch;

		readonly Rectangle _rectangle = new Rectangle(56, 0, 56, 56);

		readonly Vector2 _originalPosition;

		readonly Hose _hose;
		readonly DigDug _digDug;

		byte _growCount;

		byte[,] _level;

		State _lastState;

		Vector2 _lastSpeed,
				_digDugLastKnownPosition;

		internal Monster(int x, byte y, bool isFygar, byte multiplier, byte monsterSpeed) : base(isFygar ? Game1.FygarTexture : Game1.PookaTexture)
		{
			_isFygar = isFygar;
			_monsterSpeed = monsterSpeed;
			IsVisible = true;
			_y = y;

			_originalPosition = new Vector2(x * 56, y * 56);
			Position = _originalPosition;
			Source = _rectangle;

			_digDug = Game1.DigDug;
			_hose = _digDug.Hose;
			_digDugLastKnownPosition = _digDug.Position;

			_multiplier = multiplier;
		}

		internal override void Update()
		{
			if (!IsVisible) return;

			var (x, y) = Position;
			_level = Game1.Level;

			var fireWatchElapsed = _fireWatch.ElapsedMilliseconds;

			switch (CurrentState)
			{
				case State.Dead when _deadWatch.ElapsedMilliseconds >= 1000:
					IsVisible = false;
					_deadWatch.Stop();
					break;
				case State.Walking when fireWatchElapsed >= Random.Next(10000, 20000) && _isFygar:
					Fire.IsVisible = true;
					_walkWatch.Reset();
					CurrentState = State.Dragon;
					Fire.Position = new Vector2(x + (IsFlipped ? 56 : -56), y);
					_fireWatch.Restart();
					break;
				case State.Walking when _ghostWatch.ElapsedMilliseconds >= Random.Next(10000, 20000) && _game1Stopwatch.ElapsedMilliseconds >= 5000:
					_walkWatch.Reset();
					Source.X = 0;
					Source.Y = 0;
					CurrentState = State.Ghost;
					_ghostWatch.Reset();
					_timeAsGhost.Start();
					break;
				case State.Ghost when !(Math.Abs(x % 56) > 0 || Math.Abs(y % 56) > 0) && _level[(int) x / 56, (int) y / 56] == 0 && _timeAsGhost.ElapsedMilliseconds >= 1000:
					_timeAsGhost.Reset();
					Source.X = 56;
					CurrentState = State.Walking;
					_walkWatch.Start();
					_ghostWatch.Start();
					break;
				case State.Dragon when fireWatchElapsed >= 2000:
					Fire.IsVisible = false;
					_fireWatch.Restart();
					_walkWatch.Start();
					CurrentState = State.Walking;
					break;
				case State.Walking when Collides(_hose):
					_walkWatch.Reset();
					_lastState = CurrentState;
					Source.Y = 0;
					CurrentState = State.Growing;
					_stopwatch.Start();
					break;
				case State.Growing when _stopwatch.ElapsedMilliseconds >= 1000:
					if (Collides(_hose)) ++_growCount;
					else if (_growCount > 0) --_growCount;

					switch (_growCount)
					{
						case 0:
							CurrentState = _lastState;
							_stopwatch.Stop();
							break;
						case 4:
							Die(1);
							break;
					}

					Source.X = CurrentState == State.Ghost ? 0 : 56 * (_growCount + 1);

					_stopwatch.Restart();
					break;
			}

			UpdateMovement(x, y);
		}

		internal void Die(byte method)
		{
			_deadWatch.Start();
			++Game1.DeadMonsters;
			CurrentState = State.Dead;
			Game1.Score += (ushort) (_multiplier * method * (_y + 1) * 10);
		}

		internal void Reset()
		{
			if (CurrentState == State.Dead) return;

			Position = _originalPosition;
			CurrentState = State.Walking;
			Source = _rectangle;
			IsFlipped = false;
			Fire.IsFlipped = false;
			Fire.IsVisible = false;
			_stopwatch.Reset();
			_fireWatch.Reset();
			_ghostWatch.Reset();
			_timeAsGhost.Reset();
			_walkWatch.Reset();
			_deadWatch.Reset();
		}

		internal void Play()
		{
			_fireWatch.Start();
			_ghostWatch.Start();
			_walkWatch.Start();
		}

		void UpdateMovement(float x, float y)
		{
			if (CurrentState != State.Ghost)
			{
				var dp = _digDug.Position;

				if (Math.Abs(dp.X % 56) <= 0 && Math.Abs(dp.Y % 56) <= 0) _digDugLastKnownPosition = dp;
			}

			switch (CurrentState)
			{
				case State.Ghost:
					var (dx, dy) = _digDugLastKnownPosition;

					var monsterSpeed2 = _monsterSpeed / 2f;

					if (dx > x)
					{
						Position.X += monsterSpeed2;
						IsFlipped = true;
						Fire.IsFlipped = true;
					}
					else if (dx < x)
					{
						Position.X -= monsterSpeed2;
						IsFlipped = false;
						Fire.IsFlipped = false;
					}
					if (dy > y) Position.Y += monsterSpeed2;
					else if (dy < y) Position.Y -= monsterSpeed2;
					break;
				case State.Walking:
					if (Math.Abs(x % 56) > 0 || Math.Abs(y % 56) > 0) Position += _lastSpeed;

					else
					{
						var choose = false;

						var x1 = (int) (x / 56);
						var y1 = (int) (y / 56);

						while (!choose)
						{
							switch (Random.Next(0, 4))
							{
								case 0 when Math.Abs(x) > 0 && _level[x1 - 1, y1] == 0:
									Position.X -= _monsterSpeed;

									_lastSpeed.X = -_monsterSpeed;
									_lastSpeed.Y = 0;

									choose = true;
									break;
								case 1 when Math.Abs(x - 952) > 0 && _level[x1 + 1, y1] == 0:
									Position.X += _monsterSpeed;

									_lastSpeed.X = _monsterSpeed;
									_lastSpeed.Y = 0;

									choose = true;
									break;
								case 2 when Math.Abs(y) > 0 && _level[x1, y1 - 1] == 0:
									Position.Y -= _monsterSpeed;

									_lastSpeed.X = 0;
									_lastSpeed.Y = -_monsterSpeed;

									choose = true;
									break;
								case 3 when Math.Abs(y - 728) > 0 && _level[x1, y1 + 1] == 0:
									Position.Y += _monsterSpeed;

									_lastSpeed.X = 0;
									_lastSpeed.Y = _monsterSpeed;

									choose = true;
									break;
							}

							if (_lastSpeed.X > 0)
							{
								IsFlipped = true;
								Fire.IsFlipped = true;
							}
							else if (_lastSpeed.X < 0)
							{
								IsFlipped = false;
								Fire.IsFlipped = false;
							}
						}
					}

					var elapsed = _walkWatch.ElapsedMilliseconds;

					if (elapsed >= 500)
					{
						Source.Y = 56;
						_walkWatch.Restart();
					}
					else if (elapsed >= 250) Source.Y = 0;

					break;
			}
		}
	}
}