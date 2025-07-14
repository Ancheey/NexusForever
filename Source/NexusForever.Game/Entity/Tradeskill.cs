using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static.Crafting;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;
using NexusForever.Network.World.Message.Model.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexusForever.Game.Entity
{
    public class Tradeskill : ITradeskill
    {
        private static Dictionary<TradeskillType, List<TradeskillTierEntry>> tiersCache = [];
        public TradeskillType Type { get; }
        public uint TradeskillXp { get; set; }

        public uint PropertyProficiencyFlags { get; set; }

        public uint TalentPoints { get; set; }

        public uint[] TradeskillTalentTierIds { get; set; }

        public bool IsActive { get; set; }

        public TradeskillInfo GetInfo() => new TradeskillInfo()
        {
            IsActive = (uint)(IsActive ? 1 : 0),
            TradeskillId = Type,
            PropertyProficiencyFlags = PropertyProficiencyFlags,
            TalentPoints = TalentPoints,
            TradeskillTalentTierIds = TradeskillTalentTierIds,
            TradeskillXp = TradeskillXp
        };

        public Tradeskill(TradeskillType type)
        {
            Type = type;
            TradeskillTalentTierIds = new uint[10];
            IsActive = false;
        }
    }
}
