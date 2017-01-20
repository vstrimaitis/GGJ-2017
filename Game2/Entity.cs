using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game2
{
    class Entity
    {
        public World World { get; }
        public Vector2 Position;
        public Vector2 Size;
        public Vector2 Velocity;

        public Entity(Vector2 pos, /*Vector2 size,*/ Vector2 vel, World world)
        {
            Position = pos;
            //Size = size;
            Size = new Vector2(1, 1);
            Velocity = vel;
            World = world;
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(Graphics.Pixel, new Rectangle((int)(Position.X * Block.Size), (int)(Position.Y * Block.Size), (int)(Size.X * Block.Size), (int)(Size.Y * Block.Size)), Color.Red);
        }

        public bool IsInBlock
        {
            get
            {
                foreach(var b in World.Blocks)
                {
                    if (b.Contains(Position))
                        return true;
                }
                return false;
            }
        }

        public bool MoveUp(float dist)
        {
            float movedDist = dist;
            foreach(var b in World.Blocks)
            {
                if (b.Position.X + 1 < Position.X)
                    continue;

                if (b.Position.X > Position.X + Size.X)
                    continue;

                if (b.Position.Y + 1 - 0.001f > Position.Y)
                    continue;

                if (b.Position.Y + 1 > Position.Y - movedDist)
                {
                    movedDist = b.Position.Y + 1 - Position.Y;
                    movedDist *= -1; // ?
                }
            }
            Position.Y -= movedDist;
            return movedDist < dist;
        }
    }
}
