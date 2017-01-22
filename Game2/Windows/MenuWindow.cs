using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

namespace Game2.Windows
{
    class MenuWindow : IWindow
    {
        public int Width { get; set; }

        public int Height { get; set; }

        public event EventHandler<GameState> StateChanged;

        public MenuWindow(int w, int h)
        {
            Width = w;
            Height = h;
        }

        public void Update()
        {
            var state = Keyboard.GetState();
            if (state.IsKeyDown(Keys.Space))
            {
                // change state
            }
        }
        public void Draw(SpriteBatch sb)
        {
            throw new NotImplementedException();
        }

        public void Initialize()
        {

        }
    }
}
