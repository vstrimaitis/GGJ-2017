using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game2
{
    enum BlockType
    {
        Ground,
    }

    class Block
    {
        public const int Size = 5;
        public BlockType Type { get; private set; }
        public Color Color { get; private set; }
        public Vector2 Position { get; private set; }

        public Block(int x, int y, BlockType type, Color color)
        {
            Type = type;
            Position = new Vector2(x, y);
            Color = color;
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(Graphics.Pixel, new Rectangle((int)(Position.X * Game1.PixelSize), (int)(Position.Y * Game1.PixelSize), Game1.PixelSize, Game1.PixelSize), Color);
        }

        public bool Contains(Vector2 p)
        {
            if (p.X < Position.X)
                return false;
            if (p.X > Position.X)
                return false;
            return true; // ???
        }
    }
}
