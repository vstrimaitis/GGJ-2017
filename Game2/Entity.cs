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
        public bool IsOnGround { get; private set; }

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

        private bool IsInBlock(Vector2 point)
        {
            foreach (var b in World.Blocks)
            {
                if (b.Contains(point))
                    return true;
            }
            return false;
        }

        private void PushOut()
        {
            float min = 1f, max = 1000f;
            for(int i = 0; i < 100; i++)
            {
                float mid = (min + max) / 2;
                var newPos = Position * mid;
                if (IsInBlock(newPos))
                {
                    min = mid;
                }
                else
                {
                    max = mid;
                }
            }
            Position *= (min + max) / 2;
        }

        public void Update()
        {
            IsOnGround = false;
            Position += Velocity;
            if (IsInBlock(Position))
            {
                PushOut();
                IsOnGround = true;
                Velocity = Vector2.Zero;
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
