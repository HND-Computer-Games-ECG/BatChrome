using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Input;
using MonoGame.Extended.Tweening;

namespace BatChrome
{
    enum GameState
    {
        Starting,
        LaunchingLevel,
        Playing
    }

    enum SelectedEasing
    {
        None,
        Linear,
        Quintic,
        Bounce,
        BouncePlusRandom
    }

    public class Game1 : Game
    {
        public static readonly Random RNG = new Random();

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private readonly Tweener _tweener;

        private KeyboardStateExtended kb;

        private GameState _gameState;
        private float _launchDelay;
        private float _launchTimer;

        private Rectangle _screenRes;

        private bool _isColoured;
        private SelectedEasing _selectedEasing;

        private Texture2D _brickTex;
        private Texture2D _ballTex;

        public static Texture2D Pixel;

        private Bat bat;
        private List<Ball> balls;

        private Point gridTL, gridSpacing, gridSize;
        private List<List<GameObject>> brickGrid;

        private readonly int[,] LEVEL1 = new int[,]
        {
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
            { 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1},
            { 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1},
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1}
        };

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            _tweener = new Tweener();
        }

        protected override void Initialize()
        {
            _screenRes = GraphicsDevice.Viewport.Bounds;

            _gameState = GameState.Starting;
            _launchDelay = 2f;
            _launchTimer = _launchDelay;

            _isColoured = false;
            _selectedEasing = SelectedEasing.None;

            gridTL = new Point(100, 50);
            gridSpacing = new Point(50, 32);
            gridSize = new Point(LEVEL1.GetLength(1), LEVEL1.GetLength(0));

            brickGrid = new List<List<GameObject>>();

            balls = new List<Ball>();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            Pixel = new Texture2D(GraphicsDevice, 1, 1);
            Pixel.SetData(new[] {Color.White});

            _brickTex = Content.Load<Texture2D>(@"Art/brick");
            _ballTex = Content.Load<Texture2D>(@"Art/ball");

            InitLevel();
        }

        private void ResetDIPs()
        {
            _isColoured = false;
            _selectedEasing = SelectedEasing.None;
        }

        private void InitLevel()
        {
            _gameState = GameState.LaunchingLevel;
            InitBat();
            InitBricks();
        }

        private void InitBricks()
        {
            if (_selectedEasing == SelectedEasing.None)
            {
                for (var i = 0; i < gridSize.Y; i++)
                {
                    brickGrid.Add(new List<GameObject>());
                    for (var j = 0; j < gridSize.X; j++)
                    {
                        if (LEVEL1[i, j] != 0)
                        {
                            var loc = new Point(gridTL.X + gridSpacing.X * j, gridTL.Y + gridSpacing.Y * i);
                            var newBrick = new GameObject(loc, _brickTex);
                            if (_isColoured) newBrick.Tint = Palette.GetRandom(2);

                            brickGrid[i].Add(newBrick);
                        }
                    }
                }
            }
            else
            {
                for (var i = 0; i < gridSize.Y; i++)
                {
                    brickGrid.Add(new List<GameObject>());
                    for (var j = 0; j < gridSize.X; j++)
                    {
                        if (LEVEL1[i, j] != 0)
                        {
                            var loc = new Point(gridTL.X + gridSpacing.X * j, gridTL.Y + gridSpacing.Y * i);
                            var newBrick = new GameObject(new Point(loc.X, -32), _brickTex);
                            if (_isColoured) newBrick.Tint = Palette.GetRandom(2);

                            switch (_selectedEasing)
                            {
                                case SelectedEasing.None:
                                    throw new ArgumentOutOfRangeException();
                                case SelectedEasing.Linear:
                                    _tweener.TweenTo<GameObject, Vector2>(target: newBrick,
                                            expression: tBrick => newBrick.Position,
                                            toValue: loc.ToVector2(), duration: 1, delay: 0)
                                        .Easing(EasingFunctions.Linear);
                                    break;
                                case SelectedEasing.Quintic:
                                    _tweener.TweenTo<GameObject, Vector2>(target: newBrick,
                                            expression: tBrick => newBrick.Position,
                                            toValue: loc.ToVector2(), duration: 1, delay: 0)
                                        .Easing(EasingFunctions.QuinticIn);
                                    break;
                                case SelectedEasing.Bounce:
                                    _tweener.TweenTo<GameObject, Vector2>(target: newBrick,
                                            expression: tBrick => newBrick.Position,
                                            toValue: loc.ToVector2(), duration: 1, delay: 0)
                                        .Easing(EasingFunctions.BounceOut);
                                    break;
                                case SelectedEasing.BouncePlusRandom:
                                    _tweener.TweenTo<GameObject, Vector2>(target: newBrick,
                                            expression: tBrick => newBrick.Position,
                                            toValue: loc.ToVector2(), duration: 1, delay: (float) RNG.NextDouble())
                                        .Easing(EasingFunctions.BounceOut);
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }

                            brickGrid[i].Add(newBrick);
                        }
                    }
                }
            }

        }

        private void InitBat()
        {
            if (_selectedEasing == SelectedEasing.None)
            {
                bat = new Bat(new Point(_screenRes.Center.X, _screenRes.Bottom - 64),
                    Content.Load<Texture2D>(@"Art/bat"),
                    _screenRes);
            }
            else
            {
                bat = new Bat(new Point(_screenRes.Center.X, _screenRes.Bottom + 64), Content.Load<Texture2D>(@"Art/bat"),
                    _screenRes);

                switch (_selectedEasing)
                {
                    case SelectedEasing.None:
                        throw new ArgumentOutOfRangeException();
                    case SelectedEasing.Linear:
                        _tweener.TweenTo<Bat, Vector2>(target: bat, expression: tbat => bat.Position,
                                toValue: new Vector2(_screenRes.Center.X, _screenRes.Bottom - 64), duration: 1, delay: 0)
                            .Easing(EasingFunctions.Linear);
                        break;
                    case SelectedEasing.Quintic:
                        _tweener.TweenTo<Bat, Vector2>(target: bat, expression: tbat => bat.Position,
                                toValue: new Vector2(_screenRes.Center.X, _screenRes.Bottom - 64), duration: 1, delay: 0)
                            .Easing(EasingFunctions.QuinticIn);
                        break;
                    case SelectedEasing.Bounce:
                        _tweener.TweenTo<Bat, Vector2>(target: bat, expression: tbat => bat.Position,
                                toValue: new Vector2(_screenRes.Center.X, _screenRes.Bottom - 64), duration: 1, delay: 0)
                            .Easing(EasingFunctions.BounceOut);
                        break;
                    case SelectedEasing.BouncePlusRandom:
                        _tweener.TweenTo<Bat, Vector2>(target: bat, expression: tbat => bat.Position,
                                toValue: new Vector2(_screenRes.Center.X, _screenRes.Bottom - 64), duration: 1,
                                delay: (float) RNG.NextDouble())
                            .Easing(EasingFunctions.BounceOut);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            if (_isColoured) bat.Tint = Palette.GetColor(0);
        }

        protected override void Update(GameTime gameTime)
        {
            kb = KeyboardExtended.GetState();
            if (kb.IsKeyDown(Keys.Escape))
                Exit();

            _tweener.Update(gameTime.GetElapsedSeconds());

            switch (_gameState)
            {
                case GameState.Starting:
                    _gameState = GameState.LaunchingLevel;
                    break;
                case GameState.LaunchingLevel:
                    _launchTimer -= (float) gameTime.ElapsedGameTime.TotalSeconds;
                    if (_launchTimer < 0)
                    {
                        _gameState = GameState.Playing;
                        _launchTimer = _launchDelay;
                        balls.Add(new Ball(bat.CollRect.Center + new Point(0, -32), _ballTex, _screenRes));
                        if (_isColoured) balls.Last().Tint = Palette.GetColor(1);
                    }
                    break;
                case GameState.Playing:
                    DoPlaying(gameTime);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            base.Update(gameTime);
        }

        private void DoPlaying(GameTime gameTime)
        {
            bat.Update(gameTime);

            foreach (var ball in balls)
            {
                ball.Update(gameTime);

                #region bat / ball collision

                if (ball.CollRect.Intersects(bat.CollRect))
                {
                    var overlap = Rectangle.Intersect(ball.CollRect, bat.CollRect);
                    if (overlap.Width < overlap.Height)
                        ball.ReverseX();
                    else
                        ball.ReverseY();
                }

                #endregion

                #region Brick / ball collision

                for (var i = brickGrid.Count - 1; i >= 0; i--)
                {
                    for (var j = brickGrid[i].Count - 1; j >= 0; j--)
                    {
                        if (ball.CollRect.Intersects(brickGrid[i][j].CollRect))
                        {
                            var overlap = Rectangle.Intersect(ball.CollRect, brickGrid[i][j].CollRect);
                            if (overlap.Width < overlap.Height)
                                ball.ReverseX();
                            else
                                ball.ReverseY();

                            brickGrid[i].RemoveAt(j);
                        }
                    }
                }

                #endregion
            }

            if (kb.WasKeyJustDown(Keys.NumPad0)) Reset();

            if (kb.WasKeyJustDown(Keys.Back))
            {
                ResetDIPs();
                Reset();
            }

            if (kb.WasKeyJustDown(Keys.NumPad1)) ColourEverything();

            if (kb.WasKeyJustDown(Keys.NumPad2))
            {
                _selectedEasing = SelectedEasing.Linear;
                Reset();
            }

            if (kb.WasKeyJustDown(Keys.NumPad3))
            {
                _selectedEasing = SelectedEasing.Quintic;
                Reset();
            }

            if (kb.WasKeyJustDown(Keys.NumPad4))
            {
                _selectedEasing = SelectedEasing.Bounce;
                Reset();
            }

            if (kb.WasKeyJustDown(Keys.NumPad5))
            {
                _selectedEasing = SelectedEasing.BouncePlusRandom;
                Reset();
            }
        }

        private void Reset()
        {
            brickGrid.Clear();
            balls.Clear();


            InitLevel();
        }

        private void ColourEverything()
        {
            var usedList = 0;

            bat.Tint = Palette.GetColor(usedList++);

            foreach (var ball in balls)
            {
                ball.Tint = Palette.GetColor(usedList++);
            }

            foreach (var brickLine in brickGrid)
            {
                foreach (var brick in brickLine)
                {
                    brick.Tint = Palette.GetRandom(usedList);
                }
            }

            _isColoured = true;
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DimGray);

            _spriteBatch.Begin();
            bat.Draw(_spriteBatch);

            #region Draw bricks
            foreach (var brickLine in brickGrid)
            {
                foreach (var brick in brickLine)
                {
                    brick.Draw(_spriteBatch);
                }
            }
            #endregion

            #region Draw balls
            foreach (var ball in balls)
            {
                ball.Draw(_spriteBatch);
            }
            #endregion

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
