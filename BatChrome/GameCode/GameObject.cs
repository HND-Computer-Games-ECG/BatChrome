using System;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using SpriteBatch = Microsoft.Xna.Framework.Graphics.SpriteBatch;

namespace BatChrome
{
    class GameObject : Primitive
    {
        protected readonly Texture2D Art;

        protected Vector2 Speed { get; set; }

        protected Vector2 Stretch { get; set; }

        protected Color Tint;

        public GameObject() : base () { }

        public virtual void SetTint(Color col)
        {
            Tint = col;
        }

        public Color GetTint()
        {
            return Tint;
        }

        public GameObject(Point position, Texture2D art, float rotation = 0) 
            : this(position, art, rotation, Color.White) { }

        private GameObject(Point position, Texture2D art, float rotation, Color tint) 
            : base(new Rectangle(position, art.Bounds.Size), rotation)
        {
            Art = art;
            Tint = tint;

            Destination = Position;
            Speed = Vector2.One;
        }

        public virtual void Update(float deltaTime)
        {
            if (Destination == Position) return;

            var distance = (Destination - Position);
            var direction = distance.NormalizedCopy();
            var delta = direction * Speed * deltaTime;

            if (delta.Length() > distance.Length())
                Position = Destination;
            else
                Displace += direction * Speed * deltaTime;
        }

        public void Draw(SpriteBatch sb)
        {
            var currRect = CollRect;
            var newWidth = currRect.Width * MathHelper.Clamp(1 + Stretch.X, 0.50f, 1.50f);
            var newHeight = currRect.Height * MathHelper.Clamp(1 + Stretch.Y, 0.50f, 1.50f);

            currRect.Offset((CollRect.Width - newWidth)/2, (CollRect.Height - newHeight)/2);

            currRect.Width = (int) newWidth;
            currRect.Height = (int) newHeight;

            currRect.Offset(RotOffset);

            sb.Draw(Art, currRect, null, Tint, Rotation, RotOffset, SpriteEffects.None, 1);
            //sb.Draw(Game1.Pixel, CollRect, Color.Red * 0.25f);
        }
    }
}
