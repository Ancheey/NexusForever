using NexusForever.Game.Abstract.Entity;
using NexusForever.Script.Template;
using NexusForever.Script.Template.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexusForever.Script.Alizar.Thayd.Teleporters
{
    [ScriptFilterCreatureId(67490)]
    public class WhitevaleTeleporterEntityScript : IOwnedScript<INonPlayerEntity>, INonPlayerScript
    {
        public void OnActivateSuccess(IPlayer player)
        {
            player.TeleportTo(51, 4494.5f, -937, -572);
            player.Rotation = new System.Numerics.Vector3(-1.35f, 0, 0);
        }
    }
}
