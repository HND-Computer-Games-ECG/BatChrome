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
        public bool Flash { get; set; }
        private Color _baseColour;
        private float _flashAmount;

        public bool Jelly { get; set; }
        private float _rotationSpeed;

        public Vector2 Velocity => _velocity;

        private Vector2 _oldPos;

        private Vector2 _velocity;
        private Rectangle _screenBounds;

        public override void SetTint(Color col)
        {
            _baseColour = col;
            base.SetTint(col);
        }

        public Ball(Point position, Texture2D art, Rectangle screenRect, bool jelly, bool flash) : base(position, art)
        {
            Flash = flash;
            _baseColour = _tint;
            _flashAmount = 0;

            Jelly = jelly;
            _rotationSpeed = 0;

            _velocity = new Vector2(200, -200);
            _screenBounds = new Rectangle(screenRect.Left + art.Width / 2, screenRect.Top + art.Height / 2,
                screenRect.Right - art.Width, screenRect.Bottom - art.Height);

            _oldPos = Position;
        }

        public void Update(GameTime gt, SoundEffect wallHit)
        {
            if (_flashAmount > 0)
            {
                _flashAmount = MathHelper.Clamp(_flashAmount - (float) gt.ElapsedGameTime.TotalSeconds * 4, 0, 1);
                var r = MathHelper.Lerp(_baseColour.R, Color.White.R, _flashAmount);
                var g = MathHelper.Lerp(_baseColour.G, Color.White.G, _flashAmount);
                var b = MathHelper.Lerp(_baseColour.B, Color.White.B, _flashAmount);
                _tint = new Color((int) r, (int) g, (int) b);
            }

            Position += _velocity * (float) gt.ElapsedGameTime.TotalSeconds;

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

            if (Jelly)
            {
                if (Stretch.X <= 0)
                    Stretch = Vector2.Zero;
                else
                    Stretch -= new Vector2((float) gt.ElapsedGameTime.TotalSeconds * 16);
            }

            if (Jelly)
            {
                _rotationSpeed *= 0.97f;
                Rotation += _rotationSpeed * (float) gt.ElapsedGameTime.TotalSeconds;
            }
            else
                Rotation = 0;

            _oldPos = Position;
        }

        public void ReverseX()
        {
            _velocity.X *= -1;
            Reversal(-16);
        }

        public void ReverseY()
        {
            _velocity.Y *= -1;
            Reversal(16);
        }

        private void Reversal(float newRotSpeed)
        {
            if (Flash)
                _flashAmount = 1;

            _rotationSpeed = newRotSpeed;
            Position = _oldPos;
            if (Jelly)
                Stretch = new Vector2(2);
        }
    }
}
