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
            Speed = new Vector2(900, 32);
        }

        public void Update(GameTime gt, SelectedBat batMode, MouseStateExtended ms)
        {
            var destination = MathHelper.Clamp(ms.X, minX, maxX);

            if (batMode >= SelectedBat.Jelly)
            {
                var baseStretch = Math.Abs(Position.X - destination) / 256;
                Stretch = new Vector2(baseStretch, -baseStretch);
            }
            else
                Stretch = Vector2.Zero;

            if (batMode >= SelectedBat.Smooth)
                Destination = new Vector2(destination, Destination.Y);
            else
                Position = new Vector2(destination, Position.Y);

            base.Update(gt);
        }
    }
}
