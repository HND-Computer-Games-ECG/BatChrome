using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace BatChrome
{
    class Ball : GameObject
    {
        private Vector2 _oldPos;

        private Vector2 _velocity;
        private Rectangle _screenBounds;

        public Ball(Point position, Texture2D art, Rectangle screenRect) : base(position, art)
        {
            _velocity = new Vector2(3, -3);
            _screenBounds = new Rectangle(screenRect.Left + art.Width / 2, screenRect.Top + art.Height / 2,
                screenRect.Right - art.Width, screenRect.Bottom - art.Height);

            _oldPos = Position;
        }

        public void Update(GameTime gt, SoundEffect wallHit)
        {
            Position += _velocity;

            if (Position.X < _screenBounds.Left || Position.X > _screenBounds.Right)
            {
                ReverseX();
                wallHit?.Play();
            }

            if (Position.Y < _screenBounds.Top || Position.Y > _screenBounds.Bottom)
            {
                ReverseY();
                wallHit?.Play();
            }

            _oldPos = Position;
        }

        public void ReverseX()
        {
            _velocity.X *= -1;
            Position = _oldPos;
        }

        public void ReverseY()
        {
            _velocity.Y *= -1;
            Position = _oldPos;
        }
    }
}
