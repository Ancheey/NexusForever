using NexusForever.Game.Crafting;
using NexusForever.Game.Static.Crafting;
using NexusForever.Game.Static.Entity;
using NexusForever.GameTable;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace NexusForever.WorldServer.Network.Message.Handler.Crafting
{
    public class ClientCraftingCraftItemHandler : IMessageHandler<IWorldSession, ClientCraftingCraftItem>
    {
        public void HandleMessage(IWorldSession session, ClientCraftingCraftItem packet)
        {
            var currentCraft = new CurrentCraftInfo(packet.TradeskillSchematic2Id);

            var resultItem = GameTableManager.Instance.Item.GetEntry(currentCraft.Entry.Item2IdOutput);
            var itemStats = GameTableManager.Instance.ItemStat.GetEntry(resultItem.ItemStatId);

            var availableGroups = Enum.GetValues<CraftingCircuitSocketType>();
            //most sure way to get all properties as fusion slots have them all. Used so there's no repetition in randomness
            //we copy the values so we can manipulate it.
            var availableProperties = new List<Property>(CraftingCircuitPropertyMap.Properties[CraftingCircuitSocketType.Fusion]);

            /*packet.

            //roll a property then add a group flag based on that stat (try avoiding fusions)
            Random random = new();
            for(int i = 0; i < 5; i++)
            {
                if (itemStats.ItemStatTypeEnum[i] == GameTable.Static.ItemStatType.Craftable)
                {
                    var group = Enum.GetValues()
                    //currentCraft.Stats;
                }

            }*/
            

            var message = new ServerCraftingCurrentCraft()
            {
                TradeskillSchematic2Id = packet.TradeskillSchematic2Id,
                Stats = new NexusForever.Network.World.Message.Model.Shared.CraftStats() 
                { 
                    StatType = [Property.RatingIntensity,Property.RatingVigor,Property.RatingCriticalMitigation, 0,0], //saving this is broken
                    Unknown1 = 0,
                    Unknown2 = 0, // possibly power core Id?
                    ApSpSplit = 1 //has to stay at 0
                },
                CraftingGroupFlags = [CraftingCircuitSocketType.Logic,CraftingCircuitSocketType.Logic, CraftingCircuitSocketType.Logic, 0,0],
                SchematicCount = 1,
                Item2Id = currentCraft.Entry.Item2IdOutput,
                Unused = 0,
                UnknownArray = [0, 0, 0, 0, 0],
                DiscoveryCoordinates = new Vector2(0, 0),
                DiscoveryVectorMultiplier = new Vector2(1, 1),
                DiscoveryRadiusMultiplier = 1.0f,
                AdditiveCount = 0, //same as sockets
            };
            session.EnqueueMessageEncrypted(message);
        }
    }
}
