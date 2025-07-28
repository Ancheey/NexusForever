using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Crafting
{
    public class ClientCraftingAbandonHandler : IMessageHandler<IWorldSession, ClientCraftingAbandon>
    {
        public void HandleMessage(IWorldSession session, ClientCraftingAbandon packet)
        {
            session.Player.TradeskillManager.AbandonCurrentCraft();
        }
    }
}
