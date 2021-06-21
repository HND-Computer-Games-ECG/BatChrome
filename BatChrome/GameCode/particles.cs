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

    class Emitter
    {
        protected static Texture2D ParticleTexture;
        protected List<BaseParticle> Particles;

        public Vector2 Location { get; set; }

        public Emitter(Texture2D particleTexture, Vector2 location)
        {
            ParticleTexture = particleTexture;
            Particles = new List<BaseParticle>();

            Location = location;
        }

        public virtual void Play() { }

        public void Draw(SpriteBatch sb, float deltaTime)
        {
            for (var i = Particles.Count - 1; i >= 0; i--)
            {
                if (Particles[i].Dead)
                    Particles.RemoveAt(i);
                else
                    Particles[i].Draw(sb, deltaTime);
            }
        }
    }

    class TrailEmitter : Emitter
    {
        public TrailEmitter(Texture2D particleTexture, Vector2 location) : base(particleTexture, location) { }

        public void Play(Vector2 location, float rotation, float scale, Color tint)
        {
            Location = location;
            Particles.Add(new BaseParticle(ParticleTexture, Location, Vector2.Zero, tint, rotation, scale: scale, scaleDelta: -0.1f, opacity: 0.2f, opacityDelta: -0.005f));
        }
    }

    class ImpactEmitter : Emitter
    {
        public ImpactEmitter(Texture2D particleTexture, Vector2 location) : base(particleTexture, location) { }

        public void Play(Vector2 direction, Color tint)
        {
            direction.Normalize();

            for (var i = 0; i < 3; i++)
                Particles.Add(new BaseParticle(ParticleTexture, Location, direction.Rotate(Game1.RNG.NextSingle(-0.5f, 0.5f)) * Game1.RNG.NextSingle(0.5f, 1.5f), tint, rotationSpeed: 0.1f, scaleDelta: 0.15f, opacityDelta: -0.02f));
            for (var i = 0; i < 3; i++)
                Particles.Add(new BaseParticle(ParticleTexture, Location, direction.Rotate(Game1.RNG.NextSingle(-0.5f, 0.5f)) * Game1.RNG.NextSingle(1f, 2f), tint, rotationSpeed: 0.2f, scaleDelta: 0.05f, opacityDelta: -0.01f, timeToLive: 2f));
        }
    }

    class ShatterEmitter: Emitter
    {
        private const int MARGIN = 5;

        public ShatterEmitter(Texture2D particleTexture) : base(particleTexture, Vector2.Zero) { }

        public void Play(Rectangle field, int parts, Vector2 direction, Color tint)
        {
            direction.Normalize();
            direction *= 2;

            Particles.Add(new BaseParticle(ParticleTexture, new Vector2(field.Left + MARGIN, field.Top + MARGIN), direction.Rotate(Game1.RNG.NextSingle(-0.25f, 0.25f)), tint, rotationSpeed: Game1.RNG.NextSingle(-0.1f, 0.1f), scale: 9, scaleDelta: -0.1f, opacity: 1f, opacityDelta: -0.02f));
            Particles.Add(new BaseParticle(ParticleTexture, new Vector2(field.Right - MARGIN, field.Top + MARGIN), direction.Rotate(Game1.RNG.NextSingle(-0.25f, 0.25f)), tint, rotationSpeed: Game1.RNG.NextSingle(-0.1f, 0.1f), scale: 9, scaleDelta: -0.1f, opacity: 1f, opacityDelta: -0.02f));
            Particles.Add(new BaseParticle(ParticleTexture, new Vector2(field.Right - MARGIN, field.Bottom - MARGIN), direction.Rotate(Game1.RNG.NextSingle(-0.25f, 0.25f)), tint, rotationSpeed: Game1.RNG.NextSingle(-0.1f, 0.1f), scale: 9, scaleDelta: -0.1f, opacity: 1f, opacityDelta: -0.02f));
            Particles.Add(new BaseParticle(ParticleTexture, new Vector2(field.Left + MARGIN, field.Bottom - MARGIN), direction.Rotate(Game1.RNG.NextSingle(-0.25f, 0.25f)), tint, rotationSpeed: Game1.RNG.NextSingle(-0.1f, 0.1f), scale: 9, scaleDelta: -0.1f, opacity: 1f, opacityDelta: -0.02f));

            for (var i = 0; i < parts; i++)
            {
                var randomLoc = field.Location + new Point(Game1.RNG.Next(MARGIN, field.Width - MARGIN * 2),
                    Game1.RNG.Next(MARGIN, field.Height - MARGIN * 2));
                var size = Game1.RNG.NextSingle(4f, 13f);
                var fallSpeed = direction * size / 10;
                Particles.Add(new BaseParticle(ParticleTexture, randomLoc.ToVector2(), fallSpeed.Rotate(Game1.RNG.NextSingle(-0.25f, 0.25f)), tint, rotationSpeed: Game1.RNG.NextSingle(-0.1f, 0.1f), scale: size, scaleDelta: -0.01f, opacity: 0.75f, opacityDelta: -0.01f, timeToLive: 2));
            }
        }
    }

}
