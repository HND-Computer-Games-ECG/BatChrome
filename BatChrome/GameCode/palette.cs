using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace BatChrome
{
    public static class Palette
    {
        public static List<Color> PaletteList;

        static Palette()
        {
            #region Generate some visually distinct colours.
            PaletteList = new List<Color>
            {
                new Color(230, 25, 75, 255),    // Red
                new Color(60, 180, 75, 255),    // Green
                new Color(255, 225, 25, 255),   // Yellow
                new Color(0, 130, 200, 255),    // Blue
                new Color(245, 130, 48, 255),   // Orange
                new Color(145, 30, 180, 255),   // Purple
                new Color(70, 240, 240, 255),   // Cyan
                new Color(240, 50, 230, 255),   // Magenta
                new Color(210, 245, 60, 255),   // Lime
                new Color(250, 190, 190, 255),  // Pink
                new Color(0, 128, 128, 255),    // Teal
                new Color(230, 190, 255, 255),  // Lavender
                new Color(170, 110, 40, 255),   // Brown
                new Color(255, 250, 200, 255),  // Beige
                new Color(128, 0, 0, 255),      // Maroon
                new Color(170, 255, 195, 255),  // Mint
                new Color(128, 128, 0, 255),    // Olive
                new Color(255, 215, 180, 255),  // Apricot
                new Color(0, 0, 128, 255)       // Navy
            };
            #endregion
        }

        public static Color GetRandom(int ignoreBelow = 0)
        {
            return PaletteList[Game1.RNG.Next(ignoreBelow, PaletteList.Count)];
        }

        public static Color GetColor(int col)
        {
            return PaletteList[col];
        }
    }
}
