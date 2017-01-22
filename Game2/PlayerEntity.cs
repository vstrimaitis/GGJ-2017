﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Game2
{
    class PlayerEntity
    {
        public const int BlockSize = Game1.PlanetBlockSize;
        float ChargeSpeed;
        float DischargeSpeed;
        public World World { get; }
        public Vector2 Position;
        public Vector2 Size;
        public Vector2 Velocity;
        public Vector2 AbsolutePosition;
        public Vector2 Bounds;
        public Texture2D Texture;
        public Texture2D TextureOverlay;
        public bool IsOnGround { get; private set; }
        public float Power { get; private set; } = 100;
        public int Score;

        public List<Block> Blocks { get; } = new List<Block>();
        public Block ReferenceBlock;

        public event EventHandler OnDeath;
        
        public PlayerEntity(Vector2 pos, Vector2 vel, World world, float chargeSpeed, float dischargeSpeed, Texture2D texture, Texture2D overlay)
        {
            ChargeSpeed = chargeSpeed;
            DischargeSpeed = dischargeSpeed;
            TextureOverlay = overlay;
            Texture = texture;
            Position = pos;
            Size = new Vector2(10,10);
            Velocity = vel;
            World = world;
        }

        public void Draw(SpriteBatch sb)
        {

            var rect = new Rectangle((int)(Position.X * BlockSize), (int)(Position.Y * BlockSize), (int)(Size.X * BlockSize), (int)(Size.Y * BlockSize));
            float angle = (float)Math.Atan2(Position.Y, Position.X) + MathHelper.PiOver2;
            sb.Draw(Texture, rect, null, Color.White, angle, new Vector2(Texture.Width/2, Texture.Height), SpriteEffects.None, 0);

            var color = GraphicsHelper.Interpolate(Color.Red, new Color(0, 255, 0), Power / 100);
            sb.Draw(TextureOverlay, rect, null, color, angle, new Vector2(TextureOverlay.Width / 2, TextureOverlay.Height), SpriteEffects.None, 0);
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

            foreach (var b in Blocks)
                b.Position = Position + b.Coordinates;
            if (Game1.IsLit(Position, World.SunPosition))
                Power = MathHelper.Min(Power + ChargeSpeed, 100);
            else
                Power = MathHelper.Max(Power - DischargeSpeed, 0);
            if (Power == 0)
                OnDeath?.Invoke(this, EventArgs.Empty);
        }

        public void DrainPower(int amount)
        {
            Power -= amount;
            if (Power <= 0)
                OnDeath?.Invoke(this, EventArgs.Empty);
        }
    }
}
