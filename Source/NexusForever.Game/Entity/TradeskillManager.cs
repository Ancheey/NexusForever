using NexusForever.Database.Character;
using NexusForever.Database.Character.Model;
using NexusForever.Game.Abstract.Crafting;
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
        private ICurrentCraftInfo currentCraftInfo;
        private Dictionary<TradeskillType, List<TradeskillTierEntry>> tradeskillTiers;
        private readonly int maxActiveTradeskills = 2;
        private readonly Dictionary<TradeskillType, ITradeskill> tradeskills;
        private readonly List<TradeskillType> activeTradeskills;
        private DateTime relearnCooldownFinishTimestamp;
        private IPlayer player;
        private List<uint> learnedSchematics;

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

        public uint GrantTradeskillXp(TradeskillType type, uint exp)
        {
            var expPostAddition = tradeskills[type].TradeskillXp + exp;
            var lastTier = GetTradeskillTier(type, (uint)(GetTradeskillTierCount(type) - 1));
            var experienceCap = lastTier.RequiredXp;

            expPostAddition = Math.Max(expPostAddition, 0); //not going below 0 exp (idk if needed if using uint)
            expPostAddition = Math.Min(expPostAddition, experienceCap); //Cap experience on the required exp for the last tier

            //set exp to the calculated amount
            uint expGranted = expPostAddition - tradeskills[type].TradeskillXp;
            tradeskills[type].TradeskillXp = expPostAddition;

            //grant achievements for all tiers up to the current one
            foreach(var tier in GetTradeskillTiers(type))
            {
                if(tier.RequiredXp <= tradeskills[type].TradeskillXp)
                {
                    player.AchievementManager.CheckAchievements(player, Static.Achievement.AchievementType.TradeskillLearn, tier.Id);
                }
            }
            UpdatePlayerTradeskill(type);
            return expGranted;
        }

        public uint GrantTradeskillCraftXp(ulong schematic2Id, bool craftSuccessful)
        {
            TradeskillSchematic2Entry schematic = GameTableManager.Instance.TradeskillSchematic2.GetEntry(schematic2Id);
            TradeskillType type = (TradeskillType)schematic.TradeSkillId;
            TradeskillTierEntry tradeskillTier = GetTradeskillTier(type, schematic.Tier);

            if (craftSuccessful)
            {
                //TODO: Check first craft
                return GrantTradeskillXp(type, tradeskillTier.CraftXp);
                //TODO: grant first craft exp
            }
            else
            {
                return GrantTradeskillXp(type, tradeskillTier.FailXp);
            }
        }

        public void LearnSchematic(uint schematic2Id)
        {
            if (learnedSchematics.Contains(schematic2Id))
                throw new ArgumentException($"Player {player.CharacterId} already knows schematic {schematic2Id}.");
            var schematicInfo = GameTableManager.Instance.TradeskillSchematic2.GetEntry(schematic2Id);

            if(schematicInfo == null)
                throw new ArgumentException($"Schematic {schematic2Id} does not exist.");

            learnedSchematics.Add(schematic2Id);

            var message = new ServerAddLearnedSchematic
            {
                TradeskillId = (TradeskillType)schematicInfo.TradeSkillId,
                TradeskillSchematic2Id = schematic2Id,
                DiscoveryCoordinates = new System.Numerics.Vector2(schematicInfo.VectorX, schematicInfo.VectorY)
            };

            player.Session.EnqueueMessageEncrypted(message);
            player.AchievementManager.CheckAchievements(player, Static.Achievement.AchievementType.SchematicLearn, schematic2Id);
        }

        public void GrantTalentPoints(TradeskillType type, uint amount = 1)
        {
            tradeskills[type].TalentPoints += amount;
            UpdatePlayerTradeskill(type);
        }

        public TradeskillTierEntry GetTradeskillTier(TradeskillType type)
        {
            var tiers = GetTradeskillTierCount(type);
            //we start at 2 because 1st tier is always 0 exp
            for(uint index = 2; index <= tiers; index++)
            {
                if (GetTradeskillTier(type, index).RequiredXp > tradeskills[type].TradeskillXp)
                    return GetTradeskillTier(type, index - 1);
            }
            return GetTradeskillTier(type, 1); //Only 1 tier present
        }

        public void ResetTradeskillTalents(TradeskillType type)
        {
            for (int i = 0; i < 10; i++)
                tradeskills[type].TradeskillTalentTierIds[i] = 0;

            UpdatePlayerTradeskill(type);
        }

        public void GrantTradeskillTalent(TradeskillType type, uint bonusId, uint tier)
        {
            tradeskills[type].TradeskillTalentTierIds[tier] = bonusId;

            UpdatePlayerTradeskill(type);
        }

        public uint GetTalentResetCost(TradeskillType type)
        {
            var tier = tradeskills[type].GetHighestTalentTier();
            var tradeskillTalentTiers = GameTableManager.Instance.TradeskillTalentTier.Entries
                .Where(t => (TradeskillType)t.TradeSkillId == type)
                .OrderBy(t => t.PointsToUnlock)
                .ToList();
            return tradeskillTalentTiers[(int)tier].RespecCost;
        }

        public bool CompleteCurrentCraft()
        {
            //Calculate fail chance

            //generate an item
            return true;

        }

        public void SetCurrentCraft(ICurrentCraftInfo info)
        {
            currentCraftInfo = info;
        }

        public void AbandonCurrentCraft()
        {
            currentCraftInfo = null;
            player.Session.EnqueueMessageEncrypted(new ServerCraftingCurrentCraft()
            {
                TradeskillSchematic2Id = 0,
                SchematicCount = 0
            });
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
            //list of all tiers for all tradeskills
            tradeskillTiers = new Dictionary<TradeskillType, List<TradeskillTierEntry>>(possibleTradeskills.Length);

            learnedSchematics = new List<uint>( 1 /* count from data model */);

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
            tradeskills[TradeskillType.Cooking].TalentPoints = 20;


            //debug - no clue what's proficiency about
            //could be linked to tiers?
            //tradeskills[TradeskillType.Cooking].PropertyProficiencyFlags = 35;
        }
    }
}
