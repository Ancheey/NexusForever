using NexusForever.Game.Static.Crafting;
using NexusForever.Game.Static.Entity;
using NexusForever.GameTable;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;
using System;
using System.Numerics;

namespace NexusForever.WorldServer.Network.Message.Handler.Crafting
{
    public class ClientCraftingCraftItemHandler : IMessageHandler<IWorldSession, ClientCraftingCraftItem>
    {
        public void HandleMessage(IWorldSession session, ClientCraftingCraftItem packet)
        {
            session.Player.SendSystemMessage("cast: " + packet.ClientSpellcastUniqueId);
            session.Player.SendSystemMessage("schematic: " + packet.TradeskillSchematic2Id);
            session.Player.SendSystemMessage("station: " + packet.CraftingStationUnitId);
            session.Player.SendSystemMessage("count: " + packet.SchematicCount);

            var schematic = GameTableManager.Instance.TradeskillSchematic2.GetEntry(packet.TradeskillSchematic2Id);
            var message = new ServerCraftingCurrentCraft()
            {
                TradeskillSchematic2Id = packet.TradeskillSchematic2Id,
                Stats = new NexusForever.Network.World.Message.Model.Shared.CraftStats() 
                { 
                    StatType = [Property.RatingIntensity,Property.RatingVigor,0,0,0], //saving this is broken
                    Unknown1 = 0, //breaks stats 
                    Unknown2 = 0, //breaks stats
                    ApSpSplit = 1 //has to stay at 0
                },
                CraftingGroupFlags = [CraftingCircuitSocketType.Logic,CraftingCircuitSocketType.Logic,0,0,0],
                SchematicCount = 1,
                Item2Id = schematic.Item2IdOutput,
                Unused = 1,
                UnknownArray = [0, 0, 0, 0, 0],
                DiscoveryCoordinates = new Vector2(0, 0),
                DiscoveryVectorMultiplier = new Vector2(1, 1),
                DiscoveryRadiusMultiplier = 1.0f,
                AdditiveCount = 2, //same as sockets
            };
            session.EnqueueMessageEncrypted(message);
        }
    }
}
