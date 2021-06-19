using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BatChrome
{
    enum BrickState
    {
        Alive,
        Dying,
        Dead
    }

    class Brick : GameObject
    {
        private BrickState _state;

        public BrickState State
        {
            get => _state;
            set => _state = value;
        }

        public Brick(Point position, Texture2D art, float rotation = 0)
            : base(position, art, rotation)
        {
            _state = BrickState.Alive;
        }
    }
}
