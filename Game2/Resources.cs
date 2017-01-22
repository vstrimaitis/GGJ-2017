using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game2
{
    static class Resources
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
        public static Texture2D Sun;

        public static Song BackgroundMusic;
        public static SoundEffect StarCollectSound;

        public static SpriteFont Font;
    }
}
