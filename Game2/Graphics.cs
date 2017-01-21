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
        public static Color LightSky = new Color(214, 224, 255);
        public static Color DarkSky = new Color(2, 6, 53);

        public static Texture2D Pixel;
        public static Texture2D Planet;
        public static Texture2D Background;
        public static Texture2D Player;
        public static Texture2D PlayerHat;
        public static Texture2D[] Stars;
        public static Texture2D BatteryOutline;
        public static Texture2D BatteryFill;

        private const float UpperDarkBound = 0.7f;
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

        public static Color ModifyAlpha(Color color, float dist)
        {
            var modifier = -dist * 1 / 1.5f;
            return new Color((int)(color.R * modifier), (int)(color.G * modifier), (int)(color.B * modifier), (int)(color.A * modifier));
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

        public static float Interpolate(float start, float end, float c)
        {
            c *= 2;
            start /= 255;
            end /= 255;
            end -= start;
            if (c < 1) return end / 2 * c * c * c + start;
            c -= 2;
            return end / 2 * (c * c * c + 2) + start;
            /*c *= 2;
            start /= 255;
            end /= 255;
            end -= start;
            if (c < 1) return end / 2 * c * c * c * c + start;
            c -= 2;
            return -end / 2 * (c * c * c * c - 2) + start;*/
        }

        public static Color Interpolate(Color start, Color end, float c)
        {
            float r = Interpolate(start.R, end.R, c);
            float g = Interpolate(start.G, end.G, c);
            float b = Interpolate(start.B, end.B, c);
            return new Color(r, g, b);
        }
    }
}
