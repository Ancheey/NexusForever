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
    [ScriptFilterCreatureId(51207)]
    public class MalgraveTeleporterEntityScript : IOwnedScript<INonPlayerEntity>, INonPlayerScript
    {
        public void OnActivateSuccess(IPlayer player)
        {
            player.TeleportTo(1061, 1695, -955, 4055);
            player.Rotation = new System.Numerics.Vector3(2.8f, 0, 0);
        }
    }
}
