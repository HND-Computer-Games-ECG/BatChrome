using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.Direct2D1;
using SpriteBatch = Microsoft.Xna.Framework.Graphics.SpriteBatch;

namespace BatChrome
{
    class GameObject : Primitive
    {
        private Texture2D _art;
        public Color Tint { get; set; }

        public GameObject() : base () { }

        public GameObject(Point position, Texture2D art, float rotation = 0) 
            : this(position, art, rotation, Color.White) { }

        public GameObject(Point position, Texture2D art, float rotation, Color tint) 
            : base(new Rectangle(position, art.Bounds.Size), rotation)
        {
            _art = art;
            Tint = tint;
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(_art, Position, null, Tint, Rotation, RotOffset, 1, SpriteEffects.None, 1);
            //sb.Draw(Game1.Pixel, CollRect, Color.Red * 0.25f);
        }
    }
}
