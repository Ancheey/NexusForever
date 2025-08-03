using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexusForever.Database.Character.Model
{
    public class CharacterKnownSchematicModel
    {
        public ulong Id { get; set; }
        public uint SchematicId { get; set; }
        public byte Discovered { get; set; }
        public CharacterModel Character { get; set; }
    }
}
