using NexusForever.GameTable;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexusForever.WorldServer.Network.Message.Handler.Crafting
{
    public class ClientCraftingCraftItemWithCatalystHandler : IMessageHandler<IWorldSession, ClientCraftingCraftItemWithCatalyst>
    {
        public void HandleMessage(IWorldSession session, ClientCraftingCraftItemWithCatalyst packet)
        {
            session.Player.SendSystemMessage($"Schematic count: {packet.SchematicCount}");
            session.Player.SendSystemMessage($"Schematic id: {packet.TradeskillSchematic2Id}");
            session.Player.SendSystemMessage($"Spellcast: {packet.ClientSpellcastUniqueId}");
            session.Player.SendSystemMessage($"Catalyst: {GameTableManager.Instance.TextEnglish.GetEntry(packet.CatalystItem2Id)}");
            session.Player.SendSystemMessage($"Station: {packet.CraftingStationUnitId}");
            var craft = new ServerCraftingCurrentCraft()
            {
                TradeskillSchematic2Id = packet.TradeskillSchematic2Id,
                AdditiveCount = 3
            };
            session.EnqueueMessageEncrypted(craft);
        }
    }
}
