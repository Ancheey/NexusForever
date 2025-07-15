using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexusForever.WorldServer.Network.Message.Handler.Tradeskill
{
    public class ClientTradeskillResetTalentsHandler : IMessageHandler<IWorldSession, ClientTradeskillResetTalents>
    {
        public void HandleMessage(IWorldSession session, ClientTradeskillResetTalents packet)
        {
            var cost = session.Player.TradeskillManager.GetTalentResetCost(packet.TradeskillId);
            session.Player.CurrencyManager.CurrencySubtractAmount(Game.Static.Entity.CurrencyType.Credits, cost);
            session.Player.TradeskillManager.ResetTradeskillTalents(packet.TradeskillId);
        }
    }
}
