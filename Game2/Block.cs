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
        public const int Size = 7;
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
            sb.Draw(Graphics.Pixel, new Rectangle((int)(Position.X * Size), (int)(Position.Y * Size), Size, Size), Color);
        }

        public bool Contains(Vector2 p)
        {
            if (p.X < Position.X)
                return false;
            if (p.X > Position.X + 1)
                return false;
            if (p.Y < Position.Y)
                return false;
            if (p.Y > Position.Y + 1)
                return false;
            return true;
        }
    }
}
