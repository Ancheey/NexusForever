using NexusForever.Game.Static.Crafting;
using NexusForever.Game.Static.Entity;
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
            var talentTier = session.Player.TradeskillManager.GetHighestTalentTier(packet.TradeskillId);
            //we pay with credits for cooking, vouchers for anything else.
            var currency = packet.TradeskillId == TradeskillType.Cooking ? CurrencyType.Credits : CurrencyType.CraftingVoucher;

            session.Player.CurrencyManager.CurrencySubtractAmount(currency, talentTier.RespecCost);
            session.Player.TradeskillManager.ResetTradeskillTalents(packet.TradeskillId);
        }
    }
}
