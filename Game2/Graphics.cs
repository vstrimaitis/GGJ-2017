using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game2
{
    static class Graphics
    {
        public static Color LightSky = new Color(79, 183, 198);
        public static Color DarkSky = new Color(0, 6, 129);

        public static Texture2D Pixel;
        public static Texture2D Planet;
        public static Texture2D Background;

        private const float UpperDarkBound = 0.5f;
        private const float UpperLightBound = 0.1f;

        public static Color Modify(Color color, float correctionFactor)
        {
            if (correctionFactor > UpperLightBound)
                correctionFactor = UpperLightBound;
            if (correctionFactor < -UpperDarkBound)
                correctionFactor = -UpperDarkBound;
            if (correctionFactor > 0)
                return Lighten(color, correctionFactor);
            return Darken(color, -correctionFactor);
        }

        private static Color Lighten(Color color, float correctionFactor)
        {
            float red = (255 - color.R) * correctionFactor + color.R;
            float green = (255 - color.G) * correctionFactor + color.G;
            float blue = (255 - color.B) * correctionFactor + color.B;
            return new Color((int)red, (int)green, (int)blue, color.A);
        }

        private static Color Darken(Color color, float correctionFactor)
        {
            float red = color.R * (1-correctionFactor);
            float green = color.G * (1 - correctionFactor);
            float blue = color.B * (1 - correctionFactor);
            return new Color((int)red, (int)green, (int)blue, color.A);
        }
    }
}
