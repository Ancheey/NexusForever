using NexusForever.Game.Static.Entity;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Shared
{
    public class CraftStats : IReadable, IWritable
    {
        //Reading and writing properties in this packet, as used by crafting, requires offsetting the value by 1, as it reads and requires a value higher (for some reason)
        public Property[] StatType { get; set; } = new Property[5];
        public byte Unknown1 { get; set; }
        public byte ApSpSplit { get; set; } // Attack Power and Support Power split
        public uint Unknown2 { get; set; }

        public void Read(GamePacketReader reader)
        {
            ulong temp = reader.ReadULong();

            for (int i = 0; i < StatType.Length; i++)
            {
                StatType[i] = (Property)((temp - 1) & 0xFF); //for some reason it has to be temp - 1. Otherwise the stat is offset 1 too far.
                if (StatType[i] == (Property)255)
                    StatType[i] = 0; //clean up any index overflows

                temp >>= 8;
            }

            temp >>= 8;
            Unknown1 = (byte)(temp & 0xFF);

            temp >>= 8;
            ApSpSplit = (byte)(temp & 0xFF);

            temp >>= 8;
            Unknown2 = (uint)(temp & 0xFFFFFFFF);
        }

        public void Write(GamePacketWriter writer)
        {

            ulong temp = ApSpSplit;
            temp <<= 8;

            temp |= (ulong)(Unknown1);
            temp <<= 8;

            temp |= (ulong)Unknown2;

            for (int i = 4; i >=0; i--)
            {
                temp <<= 8;
                temp |= (ulong)(StatType[i] > 0 ? StatType[i]+1 : 0);
            }

            writer.Write(temp);
        }
    }
}
