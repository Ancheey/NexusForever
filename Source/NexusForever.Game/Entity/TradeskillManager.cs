using NexusForever.Database.Character;
using NexusForever.Database.Character.Model;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static.Crafting;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;
using NexusForever.Network.World.Message.Model;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Game.Entity
{
    public class TradeskillManager : ITradeskillManager
    {
        private Dictionary<TradeskillType, List<TradeskillTierEntry>> tradeskillTiers;
        private readonly int maxActiveTradeskills = 2;
        private readonly Dictionary<TradeskillType, ITradeskill> tradeskills;
        private readonly List<TradeskillType> activeTradeskills;
        private DateTime relearnCooldownFinishTimestamp;
        private IPlayer player;

        /// <summary>
        /// Checks whether a tradeskill of certain type can be activated
        /// </summary>
        public bool CanActivateTradeskill(TradeskillType type)
        {
            if (activeTradeskills.Count >= maxActiveTradeskills)
                return false; //already has all active slots filled

            if (tradeskills[type].IsActive)
                return false; //already active

            return true;
        }
        /// <summary>
        /// Saves the Trdeskill data to the database
        /// </summary>
        /// <param name="context"></param>
        public void Save(CharacterContext context)
        {
            //tODO: make this work
        }
        /// <summary>
        /// Deactivates a tradeskill of a certain type
        /// </summary>
        /// <returns>Whether the action was successful</returns>
        public bool DeactivateTradeskill(TradeskillType type)
        {
            if (activeTradeskills.Remove(type))
            {
                tradeskills[type].IsActive = false;
                UpdatePlayerTradeskill(type);
                return true;
            }
            return false;
        }
        /// <summary>
        /// Activates a tradeskill of a certain type. A check whether it can be done before activating is advised.
        /// </summary>
        public void ActivateTradeskill(TradeskillType type)
        {
            tradeskills[type].IsActive = true;
            activeTradeskills.Add(type);
            UpdatePlayerTradeskill(type);

            //Quest handler
            player.QuestManager.ObjectiveUpdate(Static.Quest.QuestObjectiveType.LearnTradeskill, (uint)type, 1);

            //Achievement / tech tree handler
            var tradeskillFirstTierId = GetTradeskillTier(type, 1).Id;
            player.AchievementManager.CheckAchievements(player, Static.Achievement.AchievementType.TradeskillLearn, tradeskillFirstTierId);
        }
        /// <summary>
        /// Returns a list of currently active tradeskills.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ITradeskill> GetActiveTradeskills()
        {
            foreach(var activeTradeskill in activeTradeskills)
            {
                yield return tradeskills[activeTradeskill];
            }
        }
        /// <summary>
        /// Sends a set of initial data to the player. Invoked at character login.
        /// </summary>
        public void SendInitialPackets()
        {
            var message = new ServerProfessionsLoad()
            {
                RelearnCooldown = 0
            };
            foreach (var tradeskill in tradeskills)
            {
                message.Tradeskills.Add(tradeskill.Value.GetInfo());
            }
            //TODO: load schematics etc things

            player.Session.EnqueueMessageEncrypted(message);
        }
        /// <summary>
        /// Updates client data on a certain tradeskill
        /// </summary>
        public void UpdatePlayerTradeskill(TradeskillType type)
        {
            var message = new ServerProfessionUpdate()
            {
                Tradeskill = tradeskills[type].GetInfo()
            };
            player.Session.EnqueueMessageEncrypted(message);
        }
        /// <summary>
        /// Checks whether a certain tradeskill is currently active
        /// </summary>
        public bool IsTradeskillActive(TradeskillType type)
        {
            return tradeskills[type].IsActive;
        }

        public void ResetRelearnTimer(TradeskillType unlearnedTradeskill)
        {
            relearnCooldownFinishTimestamp = DateTime.UtcNow; //do actual calculations based on exp. Find a way to get the data from the gametables
        }

        public int GetRemainingRelearnCooldown()
        {
            return (int)Math.Max(0, (relearnCooldownFinishTimestamp - DateTime.UtcNow).TotalMilliseconds);
        }

        public TradeskillTierEntry GetTradeskillTier(TradeskillType type, uint tier)
        {
            foreach(var tierEntry in tradeskillTiers[type])
            {
                if(tierEntry.Tier == tier)
                    return tierEntry;
            }
            throw new ArgumentOutOfRangeException($"Supplied tier ({tier}) was out of bounds for {type} with only {GetTradeskillTierCount(type)} tiers available");
        }

        public List<TradeskillTierEntry> GetTradeskillTiers(TradeskillType type)
        {
            return tradeskillTiers[type];
        }

        public int GetTradeskillTierCount(TradeskillType type)
        {
            return tradeskillTiers[type].Count;
        }

        public TradeskillManager(IPlayer player, CharacterModel model)
        {
            this.player = player;

            //Enum of all tradeskills (might want to remove fishing)
            TradeskillType[] possibleTradeskills = Enum.GetValues<TradeskillType>();

            //list of tradeskills data
            tradeskills = new Dictionary<TradeskillType, ITradeskill>(possibleTradeskills.Length);
            //list of active tradeskills
            activeTradeskills = new List<TradeskillType>(maxActiveTradeskills);

            tradeskillTiers = new Dictionary<TradeskillType, List<TradeskillTierEntry>>(possibleTradeskills.Length);

            foreach (var tradeskillType in possibleTradeskills)
            {
                tradeskills.Add(tradeskillType, new Tradeskill(tradeskillType));
                tradeskillTiers.Add(tradeskillType, []);
                //TODO: PH - load tradeskill stats from the db
            }

            //TODO: load active tradeskills from the db

            //We're doing this to make access to tiers for achievements/tech tree easier
            var tradeskillTierEntries = GameTableManager.Instance.TradeskillTier.Entries;
            foreach(var tier in tradeskillTierEntries)
            {
                tradeskillTiers[(TradeskillType)tier.TradeSkillId].Add(tier);
            }

            //relearn
            relearnCooldownFinishTimestamp = DateTime.UtcNow; //load from db

            //Always true
            tradeskills[TradeskillType.Cooking].IsActive = true;



            //debug
            tradeskills[TradeskillType.Cooking].PropertyProficiencyFlags = 35;
        }
    }
}
