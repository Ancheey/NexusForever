using NexusForever.Game.Static.Entity;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexusForever.WorldServer.Network.Message.Handler.Crafting
{
    public class ClientCraftingCompleteComplexCraftHandler : IMessageHandler<IWorldSession, ClientCraftingCompleteComplexCraft>
    {
        public void HandleMessage(IWorldSession session, ClientCraftingCompleteComplexCraft packet)
        {
            session.Player.SendSystemMessage("--= Complex Craft =--");
            session.Player.SendSystemMessage("Schematic:   " + packet.TradeskillSchematic2Id);
            session.Player.SendSystemMessage("Spellcast:   " + packet.ClientSpellcastUniqueId);
            session.Player.SendSystemMessage("Power core:  " + packet.PowerCoreItem2Id);
            session.Player.SendSystemMessage("AP/SP:       " + packet.ApSpSplitDelta);
            session.Player.SendSystemMessage("Stats:");
            session.Player.SendSystemMessage("- AP/SP split: "+ packet.Stats.ApSpSplit);
            if (packet.Stats.StatType.Length > 0)
            {
            session.Player.SendSystemMessage("- Stat 1:      " + packet.Stats.StatType[0]);
            session.Player.SendSystemMessage("- Unk  1:      " + packet.Stats.Unknown1);
            }
            if (packet.Stats.StatType.Length > 1)
            {
                session.Player.SendSystemMessage("- Stat 2:      " + packet.Stats.StatType[1]);
                session.Player.SendSystemMessage("- Unk  2:      " + packet.Stats.Unknown2);
            }
            session.Player.SendSystemMessage("- Stat 3:      " + packet.Stats.StatType[2]);
            session.Player.SendSystemMessage("- Stat 4:      " + packet.Stats.StatType[3]);
            session.Player.SendSystemMessage("- Stat 5:      " + packet.Stats.StatType[4]);
            string arr = "[";
            foreach(var i in packet.UnknownArray)
            {
                arr += $" {i} ";
            }
            session.Player.SendSystemMessage($"Unk Array : {arr}]");

        }
    }
}
