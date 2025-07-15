using NexusForever.Game.Static.Crafting;
using NexusForever.GameTable.Model;
using NexusForever.Network.World.Message.Model.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexusForever.Game.Abstract.Entity
{
    public interface ITradeskill
    {
        public TradeskillType Type { get; }
        public uint TradeskillXp { get; set; }
        public uint PropertyProficiencyFlags { get; set; }
        public uint TalentPoints { get; set; }
        public uint[] TradeskillTalentTierIds { get; }
        public bool IsActive { get; set; }
        /// <summary>
        /// Builds info used for update messages
        /// </summary>
        /// <returns>Built message</returns>
        public TradeskillInfo GetInfo();
        public uint GetHighestTalentTier();
        
    }
}
