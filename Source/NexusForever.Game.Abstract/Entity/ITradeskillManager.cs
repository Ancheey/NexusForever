using NexusForever.Database.Character;
using NexusForever.Game.Static.Crafting;
using NexusForever.GameTable.Model;
using NexusForever.Network.World.Message.Model;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexusForever.Game.Abstract.Entity
{
    public interface ITradeskillManager : IDatabaseCharacter
    {
        /// <summary>
        /// Returns whether it can learn tradeskill of a specific type
        /// </summary>
        /// <param name="type">Tradeskill type</param>
        /// <returns>whether it can learn tradeskill of a specific type</returns>
        public bool CanActivateTradeskill(TradeskillType type);
        /// <summary>
        /// Attempts to unlearn a tradeskill of a certain type
        /// </summary>
        /// <param name="type">type of the tradeskill to unlearn</param>
        /// <returns>whether a tradeskill was unlearned</returns>
        public bool DeactivateTradeskill(TradeskillType type);
        /// <summary>
        /// Learns a tradeskill of a certain type. A check if the player can learn the tradeskill should be made before learning
        /// </summary>
        /// <param name="type">type of the tradeskill to learn</param>
        public void ActivateTradeskill(TradeskillType type);
        /// <summary>
        /// Returns a list of learned active tradeskills
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ITradeskill> GetActiveTradeskills();
        /// <summary>
        /// Sends a packet to the user client informing it of a change in the tradeskill data
        /// </summary>
        /// <param name="type">type of the tradeskill to update</param>
        public void UpdatePlayerTradeskill(TradeskillType type);
        /// <summary>
        /// Sends initial packets (ServerProfessionsLoad) to the client. Invoked when a character is being built on login.
        /// </summary>
        public void SendInitialPackets();
        /// <summary>
        /// Checks whether a certain tradeskill is currently active
        /// </summary>
        public bool IsTradeskillActive(TradeskillType type);
        /// <summary>
        /// Resets the relearn timer to the maximum value based on the progress in the unlearned tradeskill.
        /// </summary>
        public void ResetRelearnTimer(TradeskillType unlearnedTradeskill);
        /// <summary>
        /// Returns how long the cooldown is going to be in ms
        /// </summary>
        /// <returns></returns>
        public int GetRemainingRelearnCooldown();
        /// <summary>
        /// Retrieves a specified tier of a tradeskill
        /// </summary>
        /// <param name="type">type of the tradeskill</param>
        /// <param name="tier">tier of the tradeskill</param>
        /// <returns></returns>
        public TradeskillTierEntry GetTradeskillTier(TradeskillType type, uint tier);
        /// <summary>
        /// Retrieves all tiers of a tradeskill
        /// </summary>
        /// <param name="type">tradeskill type</param>
        public List<TradeskillTierEntry> GetTradeskillTiers(TradeskillType type);
        /// <summary>
        /// Retrieves the amount of tradeskill tiers
        /// </summary>
        /// <param name="type">type of the tradeskill</param>
        public int GetTradeskillTierCount(TradeskillType type);



    }
}
