using NexusForever.Game.Crafting;
using NexusForever.Game.Spell;
using NexusForever.Game.Static.Entity;
using NexusForever.GameTable;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace NexusForever.WorldServer.Network.Message.Handler.Crafting
{
    public class ClientCraftingCompleteComplexCraftHandler : IMessageHandler<IWorldSession, ClientCraftingCompleteComplexCraft>
    {
        public void HandleMessage(IWorldSession session, ClientCraftingCompleteComplexCraft packet)
        {
            Console.WriteLine($"Schematic {packet.TradeskillSchematic2Id}");
            Console.WriteLine($"Stats   [{packet.Stats.StatType[0]},{packet.Stats.StatType[1]},{packet.Stats.StatType[2]},{packet.Stats.StatType[3]},{packet.Stats.StatType[4]}]");
            Console.WriteLine($"AP/SP(D){packet.ApSpSplitDelta}");
            Console.WriteLine($"AP/SP {packet.Stats.ApSpSplit}");
            Console.WriteLine($"Core: {packet.PowerCoreItem2Id}");
            Console.WriteLine($"PPowr(D) [{packet.PropertyPowerDelta[0]},{packet.PropertyPowerDelta[1]},{packet.PropertyPowerDelta[2]},{packet.PropertyPowerDelta[3]},{packet.PropertyPowerDelta[4]}]");
            //send a spell packet
            //store craft in the tradeskillmanager
            //make spell effect to finish craft
            session.Player.TradeskillManager.SetCurrentCraft(new CurrentCraftInfo(packet.TradeskillSchematic2Id)
            {
                Stats = packet.Stats,
            });
            session.Player.CastSpell(47372,1, new SpellParameters // 47372 - Crafting Spell - MBC - Tier 1
            {
                UserInitiatedSpellCast = true,
            });

        }
    }
}
