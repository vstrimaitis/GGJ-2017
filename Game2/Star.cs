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
        public float Visibility;

        public static event EventHandler<PlayerEntity> OnCollision;

        private Vector2 _size = Vector2.Zero;

        public Rectangle BoundingBox
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, (int)_size.X, (int)_size.Y);
            }
        }

        public Star(int x, int y, Texture2D texture, World world)
        {
            World = world;
            Position = new Vector2(x, y);
            var colors1D = new Color[texture.Width * texture.Height];
            texture.GetData(colors1D);
            int startX = texture.Width, endX = 0, startY = texture.Height, endY = 0;
            for (int xx = 0; xx < texture.Width; xx++)
            {
                for (int yy = 0; yy < texture.Height; yy++)
                {
                    var c = colors1D[xx + yy * texture.Width];
                    var block = new Block((int)(xx + Position.X), (int)(yy + Position.Y), Game1.StarBlockSize, BlockType.Star, c);
                    block.AbsolutePosition = block.Position;
                    if (c.A != 0)
                    {
                        startX = Math.Min(startX, xx);
                        endX = Math.Max(endX, xx);
                        startY = Math.Min(startY, yy);
                        endY = Math.Max(endY, yy);
                        Blocks.Add(block);
                    }
                }
            }
            _size.X = endX - startX + 1;
            _size.Y = endY - startY + 1;
        }

        public void Update()
        {
            int totalVisible = 0;
            foreach(var b in Blocks)
                if (!Game1.IsLit(b.Position, World.SunPosition))
                    totalVisible++;
            Visibility = (float)totalVisible / Blocks.Count;

            if (CheckPlayer(World.Sheriff))
                return;


            if (CheckPlayer(World.Thief))
                return;
        }

        private bool CheckPlayer(PlayerEntity p)
        {
            bool collides = false;
            foreach (var b in Blocks)
            {
                var dist = (b.AbsolutePosition - p.AbsolutePosition).Length();
                if (dist < Game1.CollisionDistance)
                {
                    collides = true;
                    break;
                }
            }

            if (collides && Visibility >= Game1.StarVisibilityThreshold)
            {
                OnCollision?.Invoke(this, p);
                return true;
            }
            return false;
        }

        public bool Intersects(Star other)
        {
            return BoundingBox.Intersects(other.BoundingBox);
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
