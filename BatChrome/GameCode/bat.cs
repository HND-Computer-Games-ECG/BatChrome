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
            Speed = new Vector2(700);
        }

        public void Update(GameTime gt, bool smoothing)
        {
            var destination = MathHelper.Clamp(MouseExtended.GetState().X, minX, maxX);

            if (smoothing)
            {
                Destination = new Vector2(destination, Position.Y);
            }
            else
            {
                Position = new Vector2(destination, Position.Y);
            }

            base.Update(gt);
        }
    }
}
