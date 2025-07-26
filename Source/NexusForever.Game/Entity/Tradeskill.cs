using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static.Crafting;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Game.Entity
{

    public class Tradeskill : ITradeskill
    {
        private static Dictionary<TradeskillType, List<TradeskillTierEntry>> tiersCache = [];
        public TradeskillType Type { get; }
        public uint TradeskillXp { get; set; }

        public uint PropertyProficiencyFlags { get; set; }

        public uint TalentPoints { get; set; }
        public bool IsActive { get; set; }
        private uint[] tradeskillTalentTierIds { get; } = new uint[10]; 
        private List<TradeskillModifierInfo> tradeskillModifiers = new();

        public TradeskillInfo GetInfo() => new()
        {
            IsActive = (uint)(IsActive ? 1 : 0),
            TradeskillId = Type,
            PropertyProficiencyFlags = PropertyProficiencyFlags,
            TalentPoints = TalentPoints,
            TradeskillTalentTierIds = tradeskillTalentTierIds,
            TradeskillXp = TradeskillXp
        };

        public Tradeskill(TradeskillType type)
        {
            Type = type;
            tradeskillTalentTierIds = new uint[10];
            IsActive = false;
        }
        public uint GetHighestTalentTier()
        {
            for(uint i = 9; i >=0; i--)
            {
                if (tradeskillTalentTierIds[i] != 0)
                    return i;
            }
            return 0u;
        }

        public void PickTalent(uint tier, uint talentTierId)
        {
            tradeskillTalentTierIds[tier] = talentTierId;
            var bonus = GameTableManager.Instance.TradeskillBonus.GetEntry(talentTierId);
            
            if (bonus.TradeskillBonusEnum00 == 0)
                return; //Never happens, but better be sure.
            
            tradeskillModifiers.Add(
                new(
                    talentTierId,
                    (CraftingModifierType)bonus.TradeskillBonusEnum00,
                    (TradeskillType)bonus.ObjectIdPrimary00,
                    (int)bonus.ObjectIdSecondary00,
                    (int)bonus.ObjectIdTertiary00,
                    bonus.Value00,
                    (int)bonus.ValueInt00));

            if (bonus.TradeskillBonusEnum01 == 0)
                return;

            tradeskillModifiers.Add(
                new(
                    talentTierId,
                    (CraftingModifierType)bonus.TradeskillBonusEnum01,
                    (TradeskillType)bonus.ObjectIdPrimary01,
                    (int)bonus.ObjectIdSecondary01,
                    (int)bonus.ObjectIdTertiary01,
                    bonus.Value01,
                    (int)bonus.ValueInt01));

            if (bonus.TradeskillBonusEnum02 == 0)
                return;

            tradeskillModifiers.Add(
                new(
                    talentTierId,
                    (CraftingModifierType)bonus.TradeskillBonusEnum02,
                    (TradeskillType)bonus.ObjectIdPrimary02,
                    (int)bonus.ObjectIdSecondary02,
                    (int)bonus.ObjectIdTertiary02,
                    bonus.Value02,
                    (int)bonus.ValueInt02));
        }

        public IReadOnlyCollection<TradeskillModifierInfo> GetModifiers()
        {
            return tradeskillModifiers.AsReadOnly();
        }

        public void ResetTalents()
        {
            for (int i = 0; i < 10; i++)
                tradeskillTalentTierIds[i] = 0;
            tradeskillModifiers.Clear();
        }
    }
}
