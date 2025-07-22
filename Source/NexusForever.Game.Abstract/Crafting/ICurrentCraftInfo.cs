using NexusForever.Game.Static.Crafting;
using NexusForever.Network.World.Message.Model.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace NexusForever.Game.Abstract.Crafting
{
    public interface ICurrentCraftInfo
    {
        public uint TradeskillSchematic2Id { get; }
        public CraftStats Stats { get; }
        public List<CraftingCircuitSocketType> CraftingGroupFlags { get; set; }
        public uint SchematicCount { get; set; }
        public uint AdditiveCount { get; set; }
        public int[] StatItemPowerModifiers { get; }
        public uint Item2Id { get; set; }
        public Vector2 DiscoveryCoordinates { get; }
        public Vector2 DiscoveryVectorMultiplier { get; }
        public float DiscoveryRadiusMultiplier { get; }
    }
}
