using NexusForever.Game.Entity;
using NexusForever.GameTable;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;
using NexusForever.Network.World.Message.Model.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexusForever.WorldServer.Network.Message.Handler.Tradeskill
{
    public class ClientTradeskillLearnHandler : IMessageHandler<IWorldSession, ClientTradeskillLearn>
    {
        public void HandleMessage(IWorldSession session, ClientTradeskillLearn packet)
        {
            //if (packet.ToLearnTradeskillId != 0 && session.Player.TradeskillManager.CanLearnTradeskill(packet.ToLearnTradeskillId))
                //session.Player.TradeskillManager.LearnTradeskill(packet.ToLearnTradeskillId);
            //if(packet.ToDropTradeskillId != 0)
                //session.Player.TradeskillManager.LearnTradeskill(packet.ToDropTradeskillId);
            Console.WriteLine(packet.ToLearnTradeskillId);
            Console.WriteLine(packet.ToDropTradeskillId);

            //var message = session.Player.TradeskillManager.BuildLoadMessage();

            var info = new TradeskillInfo()
            {
                IsActive = 0,
                TradeskillId = Game.Static.Crafting.TradeskillType.Cooking,
                PropertyProficiencyFlags = 0,
                TalentPoints = 0,
                TradeskillXp = 0,
                TradeskillTalentTierIds = [65]
            };
            var message = new ServerProfessionUpdate()
            {
                Tradeskill = info
            };
            

            var schem = new ServerAddLearnedSchematic()
            {
                TradeskillId = Game.Static.Crafting.TradeskillType.Cooking,
                TradeskillSchematic2Id = 1844,
                DiscoveryCoordinates = new System.Numerics.Vector2(0, 0)
            };
            session.EnqueueMessage(schem);
            session.EnqueueMessage(message);
            //schematics next
            //if(packet.ToLearnTradeskillId)
            //session.Player.TradeskillManager.LearnTradeskill
        }
    }
}
