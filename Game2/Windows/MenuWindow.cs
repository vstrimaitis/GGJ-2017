using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

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
                StateChanged?.Invoke(this, GameState.Playing);
            }
        }
        public void Draw(SpriteBatch sb)
        {
            sb.Begin();
            sb.Draw(Resources.MenuBackground, new Rectangle(0, 0, Width, Height), Color.White);
            GraphicsHelper.DrawString(sb, Resources.Font, "Press space to start".ToUpper(), new Rectangle(250, 6*Height/7, 360, Height/4), Color.White);
            sb.End();
        }
        public void Initialize()
        {

        }
    }
}
