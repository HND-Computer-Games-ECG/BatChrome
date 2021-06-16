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

        public override void Update(GameTime gt)
        {
            var destination = MathHelper.Clamp(MouseExtended.GetState().X, minX, maxX);
            var stretch = Math.Abs(Position.X - destination);

            Destination = new Vector2(destination, Position.Y);

            base.Update(gt);
        }
    }
}
