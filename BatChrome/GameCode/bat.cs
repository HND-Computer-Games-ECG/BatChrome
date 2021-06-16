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
            Speed = new Vector2(900);
        }

        public void Update(GameTime gt, bool smoothing, bool jelly)
        {
            var destination = MathHelper.Clamp(MouseExtended.GetState().X, minX, maxX);

            if (jelly)
            {
                var baseStretch = Math.Abs(Position.X - destination) / 256;
                Stretch = new Vector2(baseStretch, -baseStretch);
            }
            else
                Stretch = Vector2.Zero;

            if (smoothing)
                Destination = new Vector2(destination, Position.Y);
            else
                Position = new Vector2(destination, Position.Y);

            base.Update(gt);
        }
    }
}
