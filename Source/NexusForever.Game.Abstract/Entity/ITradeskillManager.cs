using NexusForever.Database.Character;
using NexusForever.Game.Static.Crafting;
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
        uint MaxProfessions { get; set; }
        ITradeskill Cooking { get; }
        ITradeskill Farmer { get; }
        ITradeskill Runecrafting { get; }
        ITradeskill Fishing { get; }

        /// <summary>
        /// Returns whether it can learn tradeskill of a specific type
        /// </summary>
        /// <param name="type">Tradeskill type</param>
        /// <returns>whether it can learn tradeskill of a specific type</returns>
        public bool CanLearnTradeskill(TradeskillType type);
        /// <summary>
        /// Attempts to unlearn a tradeskill at a specific index
        /// </summary>
        /// <param name="index">index of the tradeskill</param>
        /// <returns>whether the action was successfull</returns>
        public bool UnlearnTradeskill(uint index);
        /// <summary>
        /// Attempts to unlearn a tradeskill of a certain type
        /// </summary>
        /// <param name="type">type of the tradeskill to unlearn</param>
        /// <returns>whether a tradeskill was unlearned</returns>
        public bool UnlearnTradeskill(TradeskillType type);
        /// <summary>
        /// Learns a tradeskill of a certain type
        /// </summary>
        /// <param name="type">type of the tradeskill to learn</param>
        public void LearnTradeskill(TradeskillType type);
        /// <summary>
        /// Injects a full tradeskill into the tradeskills
        /// </summary>
        /// <param name="tradeskill"></param>
        public void LearnTradeskill(ITradeskill tradeskill);
        /// <summary>
        /// Returns a list of learned active tradeskills
        /// </summary>
        /// <returns></returns>
        public ImmutableList<ITradeskill> GetActiveTradeskills();
        public ServerProfessionsLoad BuildLoadMessage();
        public void SendInitialPackets();
    }
}
