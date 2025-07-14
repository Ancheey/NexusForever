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
            if(packet.ToDropTradeskillId != 0)
                session.Player.TradeskillManager.DeactivateTradeskill(packet.ToDropTradeskillId);

            if (packet.ToLearnTradeskillId != 0 && session.Player.TradeskillManager.CanActivateTradeskill(packet.ToLearnTradeskillId))
                session.Player.TradeskillManager.ActivateTradeskill(packet.ToLearnTradeskillId);
        }
    }
}
