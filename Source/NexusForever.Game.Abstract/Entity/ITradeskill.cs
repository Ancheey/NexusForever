using NexusForever.Game.Static.Crafting;
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
        public uint TradeskillXp { get; }
        public uint PropertyProficiencyFlags { get; }
        public uint TalentPoints { get;}
        public uint[] TradeskillTalentTierIds { get; }
        public bool IsActive();
    }
}
