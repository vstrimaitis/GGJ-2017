using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game2
{
    class World
    {
        public Vector2 SunPosition;
        public List<Block> Blocks { get; } = new List<Block>();
        //public List<Entity> Entities { get; } = new List<Entity>();
        public Entity Player;
    }
}
