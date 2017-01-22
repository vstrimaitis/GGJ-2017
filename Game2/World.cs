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
        public List<Block> PlanetBlocks { get; } = new List<Block>();
        //public List<Entity> Entities { get; } = new List<Entity>();
        public PlayerEntity Sheriff;
        public PlayerEntity Thief;
        public List<Star> Stars { get; } = new List<Star>();
        public int PlanetRadius = 0;
    }
}
