using Microsoft.Extensions.Logging;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Prerequisite;
using NexusForever.Game.Static.Crafting;
using NexusForever.Game.Static.Prerequisite;
using NexusForever.GameTable;
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
            var tradeskillTierEntry = GameTableManager.Instance.TradeskillTier.GetEntry(objectId);
            switch (comparison)
            {
                case PrerequisiteComparison.GreaterThan:
                    return // we check if the tradeskill is active and whether the tier is high enough. Greater than for some reason also means GreaterOrEqual
                        player.TradeskillManager.IsTradeskillActive((TradeskillType)tradeskillTierEntry.TradeSkillId) 
                        && player.TradeskillManager.GetTradeskillTier((TradeskillType)tradeskillTierEntry.TradeSkillId).Tier >= tradeskillTierEntry.Tier;
                case PrerequisiteComparison.Equal:
                    return // we check if the tradeskill is active and whether the tier is the same.
                        player.TradeskillManager.IsTradeskillActive((TradeskillType)tradeskillTierEntry.TradeSkillId)
                        && player.TradeskillManager.GetTradeskillTier((TradeskillType)tradeskillTierEntry.TradeSkillId) == tradeskillTierEntry;
                default:
                    {
                        log.LogWarning($"Unhandled PrerequisiteComparison {comparison} for {PrerequisiteType.TradeSkillProfession}!");
                        return false;
                    }
                    
            }
        }
    }
}
