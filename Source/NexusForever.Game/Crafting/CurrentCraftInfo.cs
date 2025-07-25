using NexusForever.Game.Abstract.Crafting;
using NexusForever.Game.Static.Crafting;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;
using NexusForever.Network.World.Message.Model.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace NexusForever.Game.Crafting
{
    public class CurrentCraftInfo : ICurrentCraftInfo
    {
        public uint TradeskillSchematic2Id { get; }
        public CraftStats Stats { get; set; } = new CraftStats();
        public List<CraftingCircuitSocketType> CraftingGroupFlags { get; } = new List<CraftingCircuitSocketType>(5);
        public uint SchematicCount { get; set; } = 1;
        public uint AdditiveCount { get; set; } = 0;
        public int[] StatItemPowerModifiers { get; } = new int[5];
        public Vector2 DiscoveryCoordinates { get; set; }
        public Vector2 DiscoveryVectorMultiplier { get; set; }
        public float DiscoveryRadiusMultiplier { get; set; }
        public TradeskillType TradeskillType { get; }
        public TradeskillSchematic2Entry Entry { get; }

        public CurrentCraftInfo(uint schematic2Id)
        {
            TradeskillSchematic2Id = schematic2Id;
            Entry = GameTableManager.Instance.TradeskillSchematic2.GetEntry(schematic2Id);
            TradeskillType = (TradeskillType) Entry.TradeSkillId;
        }
    }
}
