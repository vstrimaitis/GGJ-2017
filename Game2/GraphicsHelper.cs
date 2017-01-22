using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game2
{
    static class GraphicsHelper
    {
        private const float UpperDarkBound = 0.7f;
        private const float UpperLightBound = 0.1f;

        public static void DrawString(SpriteBatch spriteBatch, SpriteFont font, string strToDraw, Rectangle boundaries, Color color)
        {
            Vector2 size = font.MeasureString(strToDraw);

            float xScale = (boundaries.Width / size.X);
            float yScale = (boundaries.Height / size.Y);

            float scale = Math.Min(xScale, yScale);

            int strWidth = (int)Math.Round(size.X * scale);
            int strHeight = (int)Math.Round(size.Y * scale);
            Vector2 position = new Vector2();
            position.X = (((boundaries.Width - strWidth) / 2) + boundaries.X);
            position.Y = (((boundaries.Height - strHeight) / 2) + boundaries.Y);

            float rotation = 0.0f;
            Vector2 spriteOrigin = new Vector2(0, 0);
            float spriteLayer = 0.0f;
            SpriteEffects spriteEffects = new SpriteEffects();

            spriteBatch.DrawString(font, strToDraw, position, color, rotation, spriteOrigin, scale, spriteEffects, spriteLayer);
        }

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
            float red = color.R * (1 - correctionFactor);
            float green = color.G * (1 - correctionFactor);
            float blue = color.B * (1 - correctionFactor);
            return new Color((int)red, (int)green, (int)blue, color.A);
        }

        private static float Interpolate(float start, float end, float c)
        {
            c *= 2;
            start /= 255;
            end /= 255;
            end -= start;
            if (c < 1) return end / 2 * c * c * c + start;
            c -= 2;
            return end / 2 * (c * c * c + 2) + start;
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
