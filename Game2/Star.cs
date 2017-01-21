using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Game2
{
    class Star
    {
        public World World;
        public List<Block> Blocks { get; } = new List<Block>();
        public int X { get; private set; }
        public int Y { get; private set; }

        public Star(int x, int y, Texture2D texture, World world)
        {
            World = world;
            X = x;
            Y = y;
            var colors1D = new Color[texture.Width * texture.Height];
            texture.GetData(colors1D);
            for (int xx = 0; xx < texture.Width; xx++)
            {
                for (int yy = 0; yy < texture.Height; yy++)
                {
                    var c = colors1D[xx + yy * texture.Width]; ;
                    if (c.A != 0)
                        Blocks.Add(new Block(xx + X, yy + Y, Game1.StarBlockSize, BlockType.Ground, c));
                }
            }
        }

        public void Draw(SpriteBatch sb)
        {
            foreach(var b in Blocks)
            {
                if(!Game1.IsLit(b.Position, World.SunPosition))
                    b.Draw(sb);
            }
        }
    }
}
