using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Game2
{
    class PlayerEntity
    {
        const int BlockSize = Game1.PlanetBlockSize;
        const float ChargeSpeed = 1 / 20f;
        const float DischargeSpeed = 1 / 10f;
        public World World { get; }
        public Vector2 Position;
        public Vector2 Size;
        public Vector2 Velocity;
        public bool IsOnGround { get; private set; }
        public float Power { get; private set; } = 100;

        public EventHandler OnDeath;

        public PlayerEntity(Vector2 pos, /*Vector2 size,*/ Vector2 vel, World world)
        {
            Position = pos;
            //Size = size;
            Size = new Vector2(5, 5);
            Velocity = vel;
            World = world;
        }

        public void Draw(SpriteBatch sb)
        {
            var rect = new Rectangle((int)(Position.X * BlockSize), (int)(Position.Y * BlockSize), (int)(Size.X * BlockSize), (int)(Size.Y * BlockSize));
            float angle = (float)Math.Atan2(Position.Y, Position.X) + MathHelper.PiOver2;
            sb.Draw(Graphics.Player, rect, null, Color.White, angle, new Vector2(Graphics.Player.Width/2, Graphics.Player.Height), SpriteEffects.None, 0);

            var color = Graphics.Interpolate(Color.Red, new Color(0, 255, 0), Power / 100);
            sb.Draw(Graphics.PlayerHat, rect, null, color, angle, new Vector2(Graphics.Player.Width / 2, Graphics.Player.Height), SpriteEffects.None, 0);
        }

        private bool IsInBlock(Vector2 point)
        {
            foreach (var b in World.PlanetBlocks)
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
            if (Game1.IsLit(Position, World.SunPosition))
                Power = MathHelper.Min(Power + ChargeSpeed, 100);
            else
                Power = MathHelper.Max(Power - DischargeSpeed, 0);
            if (Power == 0)
                OnDeath?.Invoke(this, EventArgs.Empty);
        }

        public bool MoveUp(float dist)
        {
            float movedDist = dist;
            foreach(var b in World.PlanetBlocks)
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
