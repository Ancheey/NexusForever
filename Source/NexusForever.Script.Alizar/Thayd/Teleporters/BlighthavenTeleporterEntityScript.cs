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
    [ScriptFilterCreatureId(70383)]
    public class BlighthavenTeleporterEntityScript : IOwnedScript<INonPlayerEntity>, INonPlayerScript
    {
        public void OnActivateSuccess(IPlayer player)
        {
            player.TeleportTo(1061, 1492, -950, -4890);
            player.Rotation = new System.Numerics.Vector3(-1.6f, 0, 0);
        }
    }
}
