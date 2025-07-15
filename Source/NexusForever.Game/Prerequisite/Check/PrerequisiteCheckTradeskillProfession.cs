using Microsoft.Extensions.Logging;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Prerequisite;
using NexusForever.Game.Static.Crafting;
using NexusForever.Game.Static.Prerequisite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexusForever.Game.Prerequisite.Check
{
    [PrerequisiteCheck(PrerequisiteType.TradeSkillProfession)]
    public class PrerequisiteCheckTradeskillProfession : IPrerequisiteCheck
    {

        #region Dependency Injection

        private readonly ILogger<PrerequisiteCheckTradeskillProfession> log;

        public PrerequisiteCheckTradeskillProfession(
            ILogger<PrerequisiteCheckTradeskillProfession> log)
        {
            this.log = log;
        }

        #endregion
        public bool Meets(IPlayer player, PrerequisiteComparison comparison, uint value, uint objectId, IPrerequisiteParameters parameters)
        {
            switch(comparison)
            {
                case PrerequisiteComparison.GreaterThan:
                    return 
                        player.TradeskillManager.IsTradeskillActive((TradeskillType)objectId)                   //we check if the tradeskill is active
                        && player.TradeskillManager.GetTradeskillTier((TradeskillType)objectId).Tier > value;   //And whether the tier is high enough
                default:
                    {
                        log.LogWarning($"Unhandled PrerequisiteComparison {comparison} for {PrerequisiteType.TradeSkillProfession}!");
                        return false;
                    }
                    
            }
        }
    }
}
