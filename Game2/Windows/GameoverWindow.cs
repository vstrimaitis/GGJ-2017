using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

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
            sb.Begin();
            int kw = Width / Resources.GameOverBackground.Width;
            int kh = Height / Resources.GameOverBackground.Height;
            int k = Math.Min(kw, kh);
            int w = Resources.GameOverBackground.Width * k;
            int h = Resources.GameOverBackground.Height * k;
            sb.Draw(Resources.GameOverBackground, new Rectangle(0, 0, w, h), Color.White);
            sb.End();
        }

        public void Initialize()
        {

        }

        public void Update()
        {
            var state = Keyboard.GetState();
            if (state.IsKeyDown(Keys.R))
            {
                StateChanged?.Invoke(this, GameState.Playing);
                return;
            }
            if(state.GetPressedKeys().Length > 0)
            {
                StateChanged?.Invoke(this, GameState.Exiting);
            }
        }
    }
}
