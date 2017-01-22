using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game2.Windows
{
    interface IWindow
    {
        int Width { get; set; }
        int Height { get; set; }
        event EventHandler<GameState> StateChanged;
        void Update();
        void Draw(SpriteBatch sb);
        void Initialize();
    }
}
