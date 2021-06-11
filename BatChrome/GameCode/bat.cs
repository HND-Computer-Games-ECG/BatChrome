using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Input;

namespace BatChrome
{
    class Bat : GameObject
    {
        private float minX, maxX;

        public Bat(Point position, Texture2D art, Rectangle screenRect) : base(position, art)
        {
            minX = screenRect.Left + art.Width / 2;
            maxX = screenRect.Right - art.Width / 2;
        }

        public void Update(GameTime gt)
        {
            Position = new Vector2(MathHelper.Clamp(MouseExtended.GetState().X, minX, maxX), Position.Y);
        }
    }
}
