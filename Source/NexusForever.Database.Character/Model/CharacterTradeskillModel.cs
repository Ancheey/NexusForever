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
        public uint Xp { get; set; }
        public uint talentData { get; set; }
        public virtual CharacterModel Character { get; set; }
        
    }
}
