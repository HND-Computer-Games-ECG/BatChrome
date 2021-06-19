using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace BatChrome
{
    class BaseParticle
    {
        private static Texture2D _art;
        private Color _tint;
        private float _opacity;
        private float _opacityDelta;

        private Vector2 _position;
        private Vector2 _velocity;

        private float _rotation;
        private Vector2 _rotationCentre;
        private float _rotationDelta;

        private float _scale;
        private float _scaleDelta;

        private float _ttl;

        public bool Dead => (_ttl < 0);

        public BaseParticle() : this(null, Vector2.Zero, Vector2.One, Color.White) { }

        public BaseParticle(Texture2D art, Vector2 position, Vector2 velocity, Color tint, float rotation = 0, float rotationSpeed = 0,
            float scale = 1, float scaleDelta = 0, float timeToLive = 1, float opacity = 1, float opacityDelta = 0)
        {
            _art = art;
            _tint = tint;
            _opacity = opacity;
            _opacityDelta = opacityDelta;

            _position = position;
            _velocity = velocity;

            _rotation = rotation;
            _rotationCentre = art.Bounds.Size.ToVector2()/2;
            _rotationDelta = rotationSpeed;

            _scale = scale;
            _scaleDelta = scaleDelta;

            _ttl = timeToLive;
        }

        public void Draw(SpriteBatch sb, float deltaTime)
        {
            _opacity += _opacityDelta;
            _position += _velocity;
            _rotation += _rotationDelta;
            _scale += _scaleDelta;

            _ttl -= deltaTime;

            sb.Draw(_art, _position, null, _tint * _opacity, _rotation, _rotationCentre, _scale, SpriteEffects.None, 1);
        }
    }

    class ImpactEmitter
    {
        private static Texture2D _particleTexture;
        private List<BaseParticle> _particles;

        public Vector2 Location { get; set; }

        public ImpactEmitter(Texture2D particleTexture, Vector2 location)
        {
            _particleTexture = particleTexture;
            _particles = new List<BaseParticle>();

            Location = location;
        }

        public void Play(Vector2 direction, Color tint)
        {
            direction.Normalize();

            for (var i = 0; i < 3; i++)
                _particles.Add(new BaseParticle(_particleTexture, Location, direction.Rotate(Game1.RNG.NextSingle(-0.5f, 0.5f)) * Game1.RNG.NextSingle(0.5f, 1.5f), tint, rotationSpeed: 0.1f, scaleDelta: 0.15f, opacityDelta: -0.02f));
            for (var i = 0; i < 3; i++)
                _particles.Add(new BaseParticle(_particleTexture, Location, direction.Rotate(Game1.RNG.NextSingle(-0.5f, 0.5f)) * Game1.RNG.NextSingle(1f, 2f), tint, rotationSpeed: 0.2f, scaleDelta: 0.05f, opacityDelta: -0.01f, timeToLive: 2f));
        }

        public void Draw(SpriteBatch sb, float deltaTime)
        {
            for (var i = _particles.Count - 1; i >= 0; i--)
            {
                if (_particles[i].Dead)
                    _particles.RemoveAt(i);
                else
                    _particles[i].Draw(sb, deltaTime);
            }
        }
    }
}
