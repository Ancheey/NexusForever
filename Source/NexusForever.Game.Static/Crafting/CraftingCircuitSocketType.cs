using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexusForever.Game.Static.Crafting
{
    public enum CraftingCircuitSocketType
    {
        Air = 0b001,
        Water = 0b010,
        Earth = 0b011,
        Fire = 0b100,
        Logic = 0b101,
        Life = 0b110,
        Any = 0b111
    }
}
