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
