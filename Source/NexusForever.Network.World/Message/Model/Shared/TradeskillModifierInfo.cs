using NexusForever.Game.Static.Crafting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexusForever.Network.World.Message.Model.Shared
{
    public class TradeskillModifierInfo
    {
        public uint TradeskillTalent { get; }
        public CraftingModifierType ModifierType { get; }
        public TradeskillType TradeskillAffected { get; }
        public int ObjectIdSecondary { get; }
        public int ObjectIdTertiary { get; }
        public float ValueFloat { get; }
        public int ValueInt { get; }
        public TradeskillModifierInfo(uint tradeskillTalent, CraftingModifierType modifierType, TradeskillType tradeskillAffected, int objectIdSecondary, int objectIdTertiary, float valueFloat, int valueInt)
        {
            TradeskillTalent = tradeskillTalent;
            ModifierType = modifierType;
            TradeskillAffected = tradeskillAffected;
            ObjectIdSecondary = objectIdSecondary;
            ObjectIdTertiary = objectIdTertiary;
            ValueFloat = valueFloat;
            ValueInt = valueInt;
        }
    }
}
