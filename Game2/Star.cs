using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Game2
{
    class Star
    {
        public World World;
        public List<Block> Blocks { get; } = new List<Block>();
        public Vector2 Position;

        public static event EventHandler OnCollision;

        public Star(int x, int y, Texture2D texture, World world)
        {
            World = world;
            Position = new Vector2(x, y);
            var colors1D = new Color[texture.Width * texture.Height];
            texture.GetData(colors1D);
            for (int xx = 0; xx < texture.Width; xx++)
            {
                for (int yy = 0; yy < texture.Height; yy++)
                {
                    var c = colors1D[xx + yy * texture.Width]; ;
                    if (c.A != 0)
                        Blocks.Add(new Block((int)(xx + Position.X), (int)(yy + Position.Y), Game1.StarBlockSize, BlockType.Star, c));
                }
            }
        }

        public void Update()
        {
            foreach(var b in Blocks)
            {
                var dist = (b.AbsolutePosition - World.Player.AbsolutePosition).Length();
                if (dist < 50)
                {
                    OnCollision?.Invoke(this, EventArgs.Empty);
                    Console.WriteLine(dist);
                    return;
                }
                /*if (b.BoundingBox.Intersects(World.Player.BoundingBox))
                {
                    OnCollision?.Invoke(this, EventArgs.Empty);
                    return;
                }*/

            }
        }

        public void Draw(SpriteBatch sb)
        {
            foreach(var b in Blocks)
            {
                if(!Game1.IsLit(b.Position, World.SunPosition))
                    b.Draw(sb, b.Position.Dot(World.SunPosition) * 0.01f, modifyAlpha: true);
            }
        }
    }
}
