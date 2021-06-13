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

        private bool isColoured;

        private Texture2D _brickTex;
        private Texture2D _ballTex;

        public static Texture2D Pixel;

        private Bat bat;
        private List<Ball> balls;

        private Point gridTL, gridSpacing, gridSize;
        private List<List<GameObject>> brickGrid;

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
            _launchDelay = 3f;
            _launchTimer = _launchDelay;

            isColoured = false;

            gridTL = new Point(100, 50);
            gridSpacing = new Point(50, 32);
            gridSize = new Point(13, 6);

            brickGrid = new List<List<GameObject>>();

            balls = new List<Ball>();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            Pixel = Content.Load<Texture2D>(@"Art/pixel");

            _brickTex = Content.Load<Texture2D>(@"Art/brick");
            _ballTex = Content.Load<Texture2D>(@"Art/ball");

            InitLevel();
        }

        private void InitLevel()
        {
            _gameState = GameState.LaunchingLevel;

            #region Load Bricks

            bat = new Bat(new Point(_screenRes.Center.X, _screenRes.Bottom - 64), Content.Load<Texture2D>(@"Art/bat"),
                _screenRes);

            bat = new Bat(new Point(_screenRes.Center.X, 0), Content.Load<Texture2D>(@"Art/bat"),
                _screenRes);
            _tweener.TweenTo<Bat, Vector2>(target: bat, expression: tbat => bat.Position,
                    toValue: new Vector2(_screenRes.Center.X, _screenRes.Bottom - 64), duration: 1, delay: 0)
                .Easing(EasingFunctions.Linear);

            if (isColoured) bat.Tint = Palette.GetColor(0);

            for (var i = 0; i < gridSize.Y; i++)
            {
                brickGrid.Add(new List<GameObject>());
                for (var j = 0; j < gridSize.X; j++)
                {
                    var loc = new Point(gridTL.X + gridSpacing.X * j, gridTL.Y + gridSpacing.Y * i);
                    brickGrid[i].Add(new GameObject(loc, _brickTex));
                    if (isColoured)
                        brickGrid[i].Last().Tint = Palette.GetRandom(2);
                }
            }

            #endregion

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
                        if (isColoured) balls.Last().Tint = Palette.GetColor(1);
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

            if (kb.WasKeyJustDown(Keys.NumPad1)) ColourEverything();
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

            isColoured = true;
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
