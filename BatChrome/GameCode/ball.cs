using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BatChrome
{
    class Ball : GameObject
    {
        private Vector2 oldPos;

        private Vector2 _velocity;
        private Rectangle _screenBounds;

        public Ball(Point position, Texture2D art, Rectangle screenRect) : base(position, art)
        {
            _velocity = new Vector2(3);
            _screenBounds = new Rectangle(screenRect.Left + art.Width / 2, screenRect.Top + art.Height / 2,
                screenRect.Right - art.Width, screenRect.Bottom - art.Height);

            oldPos = Position;
        }

        public void Update(GameTime gt)
        {
            Position += _velocity;

            if (Position.X < _screenBounds.Left || Position.X > _screenBounds.Right)
            {
                ReverseX();
            }

            if (Position.Y < _screenBounds.Top || Position.Y > _screenBounds.Bottom)
            {
                ReverseY();
            }

            oldPos = Position;
        }

        public void ReverseX()
        {
            _velocity.X *= -1;
            Position = oldPos;
        }

        public void ReverseY()
        {
            _velocity.Y *= -1;
            Position = oldPos;
        }
    }
}
