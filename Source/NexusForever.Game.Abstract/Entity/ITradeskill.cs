using NexusForever.Game.Static.Crafting;
using NexusForever.GameTable.Model;
using NexusForever.Network.World.Message.Model.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexusForever.Game.Abstract.Entity
{
    public interface ITradeskill
    {
        public TradeskillType Type { get; }
        public uint TradeskillXp { get; set; }
        public uint PropertyProficiencyFlags { get; set; }
        public uint TalentPoints { get; set; }
        public bool IsActive { get; set; }
        
        /// <summary>
        /// Builds info used for update messages
        /// </summary>
        /// <returns>Built message</returns>
        public TradeskillInfo GetInfo();
        /// <summary>
        /// Determines the highest talent tier user has selected (used for relearn cost)
        /// </summary>
        /// <returns>talent tier</returns>
        public uint GetHighestTalentTier();
        /// <summary>
        /// Selects a talent in a designated tier
        /// </summary>
        /// <param name="tier">tier of the talent</param>
        /// <param name="talentTierId">Id of the talent</param>
        public void PickTalent(uint tier, uint talentTierId);
        /// <summary>
        /// Returns all affecting modifiers selected by talents
        /// </summary>
        /// <returns></returns>
        public IReadOnlyCollection<TradeskillModifierInfo> GetModifiers();
        /// <summary>
        /// Resets the talent tree.
        /// </summary>
        public void ResetTalents();
    }
}
