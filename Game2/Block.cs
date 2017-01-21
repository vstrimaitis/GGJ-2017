using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game2
{
    enum BlockType
    {
        Ground,
        Player,
        Star
    }

    class Block
    {
        public int Size { get; private set; }
        public BlockType Type { get; private set; }
        public Color Color { get; private set; }
        public Vector2 Position { get; private set; }
        public Vector2 AbsolutePosition;

        public Block(int x, int y, int size, BlockType type, Color color)
        {
            Type = type;
            Position = new Vector2(x, y);
            Color = color;
            Size = size;
        }

        public void Draw(SpriteBatch sb, float dist = 0, bool modifyAlpha = false)
        {
            var color = Color;
            if (!modifyAlpha)
                color = Resources.Modify(color, dist);
            else
                color = Resources.ModifyAlpha(color, dist);
            sb.Draw(Resources.Pixel, new Rectangle((int)(Position.X * Size), (int)(Position.Y * Size), Size, Size), color);
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
