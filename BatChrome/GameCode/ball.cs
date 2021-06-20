using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.Direct2D1;
using SpriteBatch = Microsoft.Xna.Framework.Graphics.SpriteBatch;

namespace BatChrome
{
    class Ball : GameObject
    {
        public SelectedSoundFX PlaySounds { get; set; }
        private static List<SoundEffect> _wallSounds;

        public bool Flash { get; set; }
        private Color _baseColour;
        private float _flashAmount;

        public bool Jelly { get; set; }
        private float _rotationSpeed;

        public bool Trail { get; set; }
        private TrailEmitter _trailEmitter;

        public bool ImpactEmitter { get; set; }
        private ImpactEmitter _impactEmitter;

        public Vector2 Velocity => _velocity;

        private Vector2 _oldPos;

        private Vector2 _velocity;
        private Rectangle _screenBounds;

        public override void SetTint(Color col)
        {
            _baseColour = col;
            base.SetTint(col);
        }

        public Ball(Point position, Texture2D ballArt, Texture2D particleArt, Rectangle screenRect, List<SoundEffect> fx, bool jelly, bool flash, bool trail, SelectedSoundFX sounds, bool impacts) : base(position, ballArt)
        {
            PlaySounds = sounds;
            _wallSounds = fx;

            Flash = flash;
            _baseColour = Tint;
            _flashAmount = 0;

            Jelly = jelly;
            _rotationSpeed = 0;

            Trail = trail;
            _trailEmitter = new TrailEmitter(particleArt, Position);

            ImpactEmitter = impacts;
            _impactEmitter = new ImpactEmitter(particleArt, position.ToVector2());

            _velocity = new Vector2(200, -200);
            _screenBounds = new Rectangle(screenRect.Left + ballArt.Width / 2, screenRect.Top + ballArt.Height / 2,
                screenRect.Right - ballArt.Width, screenRect.Bottom - ballArt.Height);

            _oldPos = Position;
        }

        public void Move(float deltaTime)
        {
            Position += _velocity * deltaTime;
            _impactEmitter.Location = Position;
        }

        public override void Update(float deltaTime)
        {
            if (Trail)
                _trailEmitter.Play(Position, Rotation, Art.Width, Tint);

            if (_flashAmount > 0)
            {
                _flashAmount = MathHelper.Clamp(_flashAmount - deltaTime * 4, 0, 1);
                var r = MathHelper.Lerp(_baseColour.R, Color.White.R, _flashAmount);
                var g = MathHelper.Lerp(_baseColour.G, Color.White.G, _flashAmount);
                var b = MathHelper.Lerp(_baseColour.B, Color.White.B, _flashAmount);
                Tint = new Color((int) r, (int) g, (int) b);
            }

            if (Position.X < _screenBounds.Left || Position.X > _screenBounds.Right)
            {
                if (PlaySounds == SelectedSoundFX.Simple)
                    _wallSounds[0].Play();
                else if (PlaySounds == SelectedSoundFX.Better)
                    _wallSounds[Game1.RNG.Next(1, _wallSounds.Count)].Play();

                ReverseX();
            }

            if (Position.Y < _screenBounds.Top || Position.Y > _screenBounds.Bottom)
            {
                if (PlaySounds == SelectedSoundFX.Simple)
                    _wallSounds[0].Play();
                else if (PlaySounds == SelectedSoundFX.Better)
                    _wallSounds[Game1.RNG.Next(1, _wallSounds.Count)].Play();

                ReverseY();
            }

            if (Jelly)
            {
                if (Stretch.X <= 0)
                    Stretch = Vector2.Zero;
                else
                    Stretch -= new Vector2(deltaTime * 16);
            }

            if (Jelly)
            {
                _rotationSpeed *= 0.97f;
                Rotation += _rotationSpeed * deltaTime;
            }
            else
                Rotation = 0;

            _oldPos = Position;
        }

        public void Draw(SpriteBatch sb, float deltaTime)
        {
            _trailEmitter.Draw(sb, deltaTime);
            _impactEmitter.Draw(sb, deltaTime);

            base.Draw(sb);
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
            if (ImpactEmitter)
                _impactEmitter.Play(_velocity, Tint);

            if (Flash)
                _flashAmount = 1;

            _rotationSpeed = newRotSpeed;
            Position = _oldPos;

            if (Jelly)
                Stretch = new Vector2(2);
        }
    }
}
