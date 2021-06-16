using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using SharpDX.Direct2D1;
using SpriteBatch = Microsoft.Xna.Framework.Graphics.SpriteBatch;

namespace BatChrome
{
    class GameObject : Primitive
    {
        private Texture2D _art;
        private float _destinationSnap;

        public Vector2 Destination { get; set; }
        public Vector2 Speed { get; set; }

        public Color Tint { get; set; }

        public GameObject() : base () { }

        public GameObject(Point position, Texture2D art, float rotation = 0) 
            : this(position, art, rotation, Color.White) { }

        public GameObject(Point position, Texture2D art, float rotation, Color tint) 
            : base(new Rectangle(position, art.Bounds.Size), rotation)
        {
            _art = art;
            Tint = tint;

            Destination = Position;
            _destinationSnap = 1f;
            Speed = Vector2.One;
        }

        public virtual void Update(GameTime gt)
        {
            if (Destination == Position) return;

            var distance = (Destination - Position);
            var direction = distance.NormalizedCopy();
            var delta = direction * Speed * (float) gt.ElapsedGameTime.TotalSeconds;

            if (delta.Length() > distance.Length())
                Position = Destination;
            else
                Position += direction * Speed * (float) gt.ElapsedGameTime.TotalSeconds;
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(_art, CollRect, Tint);
            //sb.Draw(Game1.Pixel, CollRect, Color.Red * 0.25f);
        }
    }
}
