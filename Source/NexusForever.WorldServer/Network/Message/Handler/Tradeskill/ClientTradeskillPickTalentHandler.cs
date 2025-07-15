using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexusForever.WorldServer.Network.Message.Handler.Tradeskill
{
    public class ClientTradeskillPickTalentHandler : IMessageHandler<IWorldSession, ClientTradeskillPickTalent>
    {
        public void HandleMessage(IWorldSession session, ClientTradeskillPickTalent packet)
        {
            session.Player.TradeskillManager.GrantTradeskillTalent(packet.TradeskillId,packet.TradeskillBonusId,packet.Tier);
        }
    }
}
