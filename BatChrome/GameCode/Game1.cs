using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BatChrome
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Rectangle _screenRes;

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
        }

        protected override void Initialize()
        {
            _screenRes = GraphicsDevice.Viewport.Bounds;

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

            for (var i = 0; i < gridSize.Y; i++)
            {
                brickGrid.Add(new List<GameObject>());
                for (var j = 0; j < gridSize.X; j++)
                {
                    var loc = new Point(gridTL.X + gridSpacing.X * j, gridTL.Y + gridSpacing.Y * i);
                    brickGrid[i].Add(new GameObject(loc, _brickTex));
                }
            }

            bat = new Bat(new Point(_screenRes.Center.X, _screenRes.Bottom - 64), Content.Load<Texture2D>(@"Art/bat"),
                _screenRes);
            balls.Add(new Ball(bat.CollRect.Center + new Point(0, -32), _ballTex, _screenRes));
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            bat.Update(gameTime);
            foreach (var ball in balls)
            {
                ball.Update(gameTime);

                if (ball.CollRect.Intersects(bat.CollRect))
                {
                    var overlap = Rectangle.Intersect(ball.CollRect, bat.CollRect);
                    if (overlap.Width < overlap.Height)
                        ball.ReverseX();
                    else
                        ball.ReverseY();
                }

                for (var i = brickGrid.Count - 1; i >= 0; i--)
                {
                    for (int j = brickGrid[i].Count - 1; j >= 0; j--)
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
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DimGray);

            _spriteBatch.Begin();
            bat.Draw(_spriteBatch);

            foreach (var brickLine in brickGrid)
            {
                foreach (var brick in brickLine)
                {
                    brick.Draw(_spriteBatch);
                }
            }

            foreach (var ball in balls)
            {
                ball.Draw(_spriteBatch);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
