﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
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
        BouncePlusRandom,
        Length
    }

    enum SelectedSoundFX
    {
        None,
        Simple,
        Better,
        Length
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
        private Vector2 _uiTL;

        #region DIPs
        private bool _isColoured;
        private SelectedSoundFX _selectedFX;
        private int _hitTone;
        private SelectedEasing _selectedEasing;
        private bool _smoothBat;
        private bool _jellyBat;
        private bool _batStagger;
        private bool _jellyBall;
        private bool _flashyBall;
        #endregion

        #region Art Sources
        private Texture2D _brickTex;
        private Texture2D _ballTex;
        private SpriteFont _uiFont;

        public static Texture2D Pixel;
        #endregion

        private List<SoundEffect> _hitsFX;

        #region GameObjects
        private Bat bat;
        private List<Ball> balls;

        private Point gridTL, gridSpacing, gridSize;
        private List<List<GameObject>> brickGrid;

        private readonly int[,] LEVEL1 = 
        {
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
            { 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1},
            { 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1},
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1}
        };
        #endregion


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
            _uiTL = new Vector2(8, _screenRes.Bottom - 64);

            _gameState = GameState.Starting;
            _launchDelay = 2f;
            _launchTimer = _launchDelay;

            _hitsFX = new List<SoundEffect>();
            _hitTone = 0;

            ResetDIPs();

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
            _uiFont = Content.Load<SpriteFont>("PixelFont");

            for (int i = 0; i < 4; i++)
                _hitsFX.Add(Content.Load<SoundEffect>(@"FX/pingpong_" + i));
            _hitsFX.Add(Content.Load<SoundEffect>(@"FX/correct"));

            InitLevel();
        }

        private void ResetDIPs()
        {
            _isColoured = false;
            _selectedEasing = SelectedEasing.None;
            _smoothBat = false;
            _jellyBat = false;
            _selectedFX = SelectedSoundFX.None;
            _batStagger = false;
            _jellyBall = false;
            _flashyBall = false;
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
                            if (_isColoured) newBrick.SetTint(Palette.GetRandom(2));

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
                            if (_isColoured) newBrick.SetTint(Palette.GetRandom(2));

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

            if (_isColoured) bat.SetTint(Palette.GetColor(0));
        }

        void brickHit()
        {
            switch (_selectedFX)
            {
                case SelectedSoundFX.None:
                    break;
                case SelectedSoundFX.Simple:
                    _hitsFX[0].Play();
                    break;
                case SelectedSoundFX.Better:
                    _hitsFX[_hitTone].Play();
                    if (_hitTone < _hitsFX.Count - 1)
                        _hitTone++;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        void batHit()
        {
            switch (_selectedFX)
            {
                case SelectedSoundFX.None:
                    break;
                case SelectedSoundFX.Simple:
                case SelectedSoundFX.Better:
                    _hitTone = 0;
                    _hitsFX[_hitTone].Play();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
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
                        balls.Add(new Ball(bat.CollRect.Center + new Point(0, -32), _ballTex, _screenRes, _jellyBall, _flashyBall));
                        if (_isColoured) balls.Last().SetTint(Palette.GetColor(1));
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
            bat.Update(gameTime, _smoothBat, _jellyBat);

            foreach (var ball in balls)
            {
                #region bat / ball collision

                if (ball.CollRect.Intersects(bat.CollRect))
                {
                    var overlap = Rectangle.Intersect(ball.CollRect, bat.CollRect);
                    if (overlap.Width < overlap.Height)
                        ball.ReverseX();
                    else
                    {
                        if (_batStagger)
                            bat.Displace += ball.Velocity.NormalizedCopy() * 16;
                        ball.ReverseY();
                    }

                    batHit();
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
                            brickHit();
                            break;
                        }
                    }
                }
                #endregion

                if (_selectedFX == SelectedSoundFX.None)
                    ball.Update(gameTime, null);
                else
                    ball.Update(gameTime, _hitsFX[_hitTone]);
            }

            #region Key handlers
            if (kb.WasKeyJustDown(Keys.Back))
            {
                ResetDIPs();
                Reset();
            }

            if (kb.WasKeyJustDown(Keys.NumPad0)) Reset();

            if (kb.WasKeyJustDown(Keys.NumPad1)) ColourEverything();

            if (kb.WasKeyJustDown(Keys.NumPad2))
                _selectedFX = (SelectedSoundFX) ((int) (_selectedFX + 1) % (int) SelectedSoundFX.Length);

            if (kb.WasKeyJustDown(Keys.NumPad3))
            {
                _selectedEasing = (SelectedEasing) ((int) (_selectedEasing + 1) % (int) SelectedEasing.Length);
                Reset();
            }

            if (kb.WasKeyJustDown(Keys.NumPad4))
            {
                _smoothBat = !_smoothBat;
                if (!_smoothBat)
                    _batStagger = false;
            }

            if (kb.WasKeyJustDown(Keys.NumPad5))
                _jellyBat = !_jellyBat;

            if (_smoothBat && kb.WasKeyJustDown(Keys.NumPad6))
                _batStagger = !_batStagger;

            if (kb.WasKeyJustDown(Keys.NumPad7))
            {
                _jellyBall = !_jellyBall;
                foreach (var ball in balls)
                    ball.Jelly = _jellyBall;
            }

            if (kb.WasKeyJustDown(Keys.NumPad8))
            {
                _flashyBall = !_flashyBall;
                foreach (var ball in balls)
                    ball.Flash = _flashyBall;
            }

            #endregion

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

            bat.SetTint(Palette.GetColor(usedList++));

            foreach (var ball in balls)
            {
                ball.SetTint(Palette.GetColor(usedList++));
            }

            foreach (var brickLine in brickGrid)
            {
                foreach (var brick in brickLine)
                {
                    brick.SetTint(Palette.GetRandom(usedList));
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

            string[] uiText =
            {
                "Bsp: Reset",
                "NP0: Refresh",
                "NP1: Colour",
                "NP2: Sound-",
                "NP3: Easing-",
                "NP4: Smooth-",
                "NP5: Jellybat-",
                "NP6: Stagger-",
                "NP7: Jellyball-",
                "NP8: Flashyball-",
            };

            int i, x = 0, y = 0;
            for (i = 0; i < 3; i++, y++)
                _spriteBatch.DrawString(_uiFont, uiText[i], _uiTL + new Vector2(x, y * _uiFont.LineSpacing), Color.White);

            x += 80; y = 0;
            _spriteBatch.DrawString(_uiFont, uiText[i++] + _selectedFX, _uiTL + new Vector2(x, y), Color.White);

            y += _uiFont.LineSpacing;
            _spriteBatch.DrawString(_uiFont, uiText[i++] + _selectedEasing, _uiTL + new Vector2(x, y), Color.White);

            y += _uiFont.LineSpacing;
            _spriteBatch.DrawString(_uiFont, uiText[i++] + _smoothBat, _uiTL + new Vector2(x, y), Color.White);

            y += _uiFont.LineSpacing;
            _spriteBatch.DrawString(_uiFont, uiText[i++] + _jellyBat, _uiTL + new Vector2(x, y), Color.White);

            y += _uiFont.LineSpacing;
            _spriteBatch.DrawString(_uiFont, uiText[i++] + _batStagger, _uiTL + new Vector2(x, y), Color.White);

            x += 170; y = 0;
            _spriteBatch.DrawString(_uiFont, uiText[i++] + _jellyBall, _uiTL + new Vector2(x, y), Color.White);

            y += _uiFont.LineSpacing;
            _spriteBatch.DrawString(_uiFont, uiText[i++] + _flashyBall, _uiTL + new Vector2(x, y), Color.White);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
