using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexusForever.Database.Character.Model
{
    public class CharacterTradeskillModel
    {
        public ulong Id { get; set; }
        public ushort TradeskillId { get; set; }
        public byte IsActive { get; set; }
        public uint TradeskillXp { get; set; }
        public uint TalentData { get; set; }
        public uint ProficiencyFlags { get; set; }
        public CharacterModel Character { get; set; }
        
    }
}
