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
    [ScriptFilterCreatureId(47459)]
    public class WilderrunTeleporterEntityScript : IOwnedScript<INonPlayerEntity>, INonPlayerScript
    {
        public void OnActivateSuccess(IPlayer player)
        {
            player.TeleportTo(22, 2084, -843, -1625);
            player.Rotation = new System.Numerics.Vector3(-0.5f, 0, 0);
        }
    }
}
