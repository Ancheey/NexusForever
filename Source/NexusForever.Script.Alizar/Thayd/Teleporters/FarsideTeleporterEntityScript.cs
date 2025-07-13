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

    [ScriptFilterCreatureId(67487)]
    public class FarsideTeleporterEntityScript :IOwnedScript<INonPlayerEntity>, INonPlayerScript
    {
        public void OnActivateSuccess(IPlayer player)
        {
            player.TeleportTo(1421, 4444.5f, -712.5f, -5660.5f);
            player.Rotation = new System.Numerics.Vector3(1.226f, 0, 0);
        }
    }
}
