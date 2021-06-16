using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace BatChrome
{
    class Primitive
    {
        // Private Member Fields
        private Rectangle _rectangle;
        private Vector2 _rotationOffset;
        private Vector2 _position;

        public float Stretch { get; set; }

        // Public Properties
        public Vector2 Position
        {
            get => _position;
            set
            {
                _position = value;
                _rectangle.Location = (_position - _rotationOffset).ToPoint();
            }
        }

        public Rectangle CollRect => _rectangle;

        public Vector2 RotOffset => _rotationOffset;

        public float Rotation { get; set; }

        // Constructors
        public Primitive() : this(Rectangle.Empty) { }

        public Primitive(Vector2 position, float rotation = 0f)
        {
            _rectangle = Rectangle.Empty;
            _rotationOffset = Vector2.Zero;
            _position = position;
            Rotation = rotation;
        }

        public Primitive(Rectangle rectangle, float rotation = 0f)
        {
            _rectangle = rectangle;
            _rotationOffset = _rectangle.Size.ToVector2() / 2;
            _position = rectangle.Location.ToVector2();
            _rectangle.Location = (_position - _rotationOffset).ToPoint();
            Rotation = rotation;
        }
    }
}
