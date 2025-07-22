using NexusForever.Game.Static.Entity;
using NexusForever.GameTable;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;
using NexusForever.Network.World.Message.Model.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexusForever.WorldServer.Network.Message.Handler.Crafting
{
    public class ClientCraftingCraftItemWithCatalystHandler : IMessageHandler<IWorldSession, ClientCraftingCraftItemWithCatalyst>
    {
        //cooking craft
        public void HandleMessage(IWorldSession session, ClientCraftingCraftItemWithCatalyst packet)
        {
            session.Player.SendSystemMessage("schematic: " + packet.TradeskillSchematic2Id);
            session.Player.SendSystemMessage("castId: " + packet.ClientSpellcastUniqueId);
            session.Player.SendSystemMessage("count: " + packet.SchematicCount);
            session.Player.SendSystemMessage("catalyst: " + packet.CatalystItem2Id);
            
            /*var schematic = GameTableManager.Instance.TradeskillSchematic2.GetEntry(packet.TradeskillSchematic2Id);
            var tradeskill = GameTableManager.Instance.Tradeskill.GetEntry(schematic.TradeSkillId);
            var craft = new ServerCraftingCurrentCraft()
            {
                TradeskillSchematic2Id = packet.TradeskillSchematic2Id,
                //AdditiveCount = 0,// tradeskill.MaxAdditives,
                //Item2Id = 0,//schematic.Item2IdOutput,
                //Unused = packet.CatalystItem2Id, //idk
                //SchematicCount = 1,
                //CraftingGroupFlags = 0,
                //Stats = new CraftStats()
                //{
                 //   ApSpSplit = 1,
                 //   StatType = [],
                //   Unknown1 = 1, //charge delta
                //    Unknown2 = 0  //crafting groups?
                //},
                //StatItemPowerModifier = [1]
                //DiscoveryRadiusMultiplier = 1,
                //DiscoveryCoordinates = new System.Numerics.Vector2(0,0),
                //DiscoveryVectorMultiplier = new System.Numerics.Vector2(0,0)

            };*/
            //session.EnqueueMessageEncrypted(craft);
        }
    }
}
