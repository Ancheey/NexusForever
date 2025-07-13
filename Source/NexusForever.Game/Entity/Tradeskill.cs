using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static.Crafting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexusForever.Game.Entity
{
    public class Tradeskill : ITradeskill
    {
        public TradeskillType Type { get; }
        

        public uint TradeskillXp => tradeskillXp;

        public uint PropertyProficiencyFlags { get; }

        public uint TalentPoints => talentPoints;

        public uint[] TradeskillTalentTierIds { get; }

        public bool IsActive()
        {
            return Type switch
            {
                TradeskillType.Cooking => false,
                TradeskillType.Farmer => false,
                TradeskillType.Fishing => false,
                TradeskillType.Runecrafting => false,
                _ => true
            };
        }
        public Tradeskill(TradeskillType type)
        {
            Type = type;
            TradeskillTalentTierIds = new uint[10];
            //PropertyProficiencyFlags = 0;
            if (type == TradeskillType.Cooking)
            {
                TradeskillTalentTierIds[0] = 40;
                TradeskillTalentTierIds[1] = 41;
                TradeskillTalentTierIds[2] = 42;
                TradeskillTalentTierIds[3] = 43;
                TradeskillTalentTierIds[4] = 44;
                TradeskillTalentTierIds[5] = 45;
                TradeskillTalentTierIds[6] = 46;
                TradeskillTalentTierIds[7] = 47;
                PropertyProficiencyFlags = 40;
            }
        }
        private uint tradeskillXp = 0;
        private uint talentPoints = 1;
    }
}
