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
    [ScriptFilterCreatureId(70385)]
    public class DefileTeleporterEntityScript : IOwnedScript<INonPlayerEntity>, INonPlayerScript
    {
        public void OnActivateSuccess(IPlayer player)
        {
            player.TeleportTo(1061, 4121, -966, -5088);
            player.Rotation = new System.Numerics.Vector3(0.1f, 0, 0);
        }
    }
}
