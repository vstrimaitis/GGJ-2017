using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

namespace Game2.Windows
{
    class GameoverWindow : IWindow
    {
        public int Height { get; set; }

        public int Width { get; set; }

        public event EventHandler<GameState> StateChanged;

        public GameoverWindow(int w, int h)
        {
            Width = w;
            Height = h;
        }

        public void Draw(SpriteBatch sb)
        {

        }

        public void Initialize()
        {

        }

        public void Update()
        {

        }
    }
}
