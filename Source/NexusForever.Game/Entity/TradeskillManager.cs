using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using NexusForever.Database.Character;
using NexusForever.Database.Character.Model;
using NexusForever.Game.Abstract.Crafting;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static.Crafting;
using NexusForever.Game.Static.Entity;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;
using NexusForever.GameTable.Static;
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
        private readonly IPlayer player;
        private List<uint> learnedSchematics;
        private readonly List<TradeskillModifierInfo> globalModifiers = [];

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
                RelearnCooldown = 0,
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
            var pricePoint = tradeskills[type].GetHighestTalentTier();
            tradeskills[type].ResetTalents();
            player.Session.EnqueueMessageEncrypted(getModifiersMessage());
            UpdatePlayerTradeskill(type);
        }

        public void GrantTradeskillTalent(TradeskillType type, uint bonusId, uint tier)
        {
            tradeskills[type].PickTalent(tier, bonusId);
            player.Session.EnqueueMessageEncrypted(getModifiersMessage());
            UpdatePlayerTradeskill(type);
        }
        private ServerProfessionModifiers getModifiersMessage()
        {
            //this method could use caching as it might be a slog
            var message = new ServerProfessionModifiers();

            // Gather all modifiers
            List<TradeskillModifierInfo> modifiers = tradeskills.Keys
                .SelectMany(k => tradeskills[k].GetModifiers())
                .OrderBy(m=>m.ModifierType)
                .ToList();

            // get unique types
            List<CraftingModifierType> distinctModifiers = modifiers
                .Select(m => m.ModifierType)
                .Distinct()
                .ToList();

            // create a header for each modifier
            foreach(var type in distinctModifiers)
            {
                message.Modifiers.Add(new ServerProfessionModifiers.CraftingModifier
                {
                    Type = type,
                    Coefficient = 0,
                });
                var typeMods = modifiers.Where(m => m.ModifierType == type);
                foreach (var modifier in typeMods)
                {
                    message.Modifiers.Add(new ServerProfessionModifiers.CraftingModifier
                    {
                        Type = modifier.ModifierType,
                        TradeskillType = modifier.TradeskillAffected,
                        Item2TypeId = modifier.ObjectIdSecondary,
                        Item2MaterialId = modifier.ObjectIdTertiary,
                        Coefficient = modifier.ValueFloat,
                        FixedValue = modifier.ValueInt
                    });
                }
            }
            
            return message;
        }
        public bool CompleteCurrentCraft()
        {
            var isSuccessful = true;
            switch (currentCraftInfo.TradeskillType)
            {
                case TradeskillType.Cooking:
                    throw new NotImplementedException();
                case TradeskillType.Architect:
                    throw new NotImplementedException();
                default:
                    isSuccessful = completeCircuitCurrentCraft(currentCraftInfo);
                    break;
            }
            return isSuccessful;
        }
        private bool completeCircuitCurrentCraft(ICurrentCraftInfo currentCraft)
        {
            var rand = new Random();
            var failChance = calculateFailChance(                           // Get the fail chance
                [.. currentCraft.CraftingGroupFlags],
                currentCraft.Stats.StatType,
                currentCraft.StatItemPowerModifiers);
            failChance *= 100;                                              // Move it from 0.x to 0-100
            failChance = (float)Math.Floor((decimal)failChance);            // Floor the value like it's done in the client.

            bool isSuccessful = rand.Next(0, 101) > failChance;             // Check if a random roll from a 0 to 100 is higher than failchance

            //generate a reward
            GrantTradeskillCraftXp(currentCraft.TradeskillSchematic2Id, isSuccessful);
            //generate item

            //check achievements and quests

            return false;
        }
        private float calculateFailChance(CraftingCircuitSocketType[] groupFlags, Property[] properties, int[] powerDeltas)
        {
            //this is overall nearly 99% accurate. Only excaptions are with very low percentages

            float baseFailChance = 0.2925f;                                 // Base fail chance for 1 socket
            var socketCount = groupFlags.Count(cgf => cgf != 0);            // Socket count

            baseFailChance /= socketCount > 0 ? socketCount : 1;            // Calculate the % per socket power modifier

            //mismatched sockets
            float MismatchPenalty = 0.78f;                              // base TOTAL penalty for mismatching 

            float baseMismatchPenaltyReduction = GetModifierValue(CraftingModifierType.MismatchPenalty);
            MismatchPenalty *= baseMismatchPenaltyReduction;            // Penalty reduction

            float MismatchFailChance = 0.375f;
            MismatchFailChance /= socketCount > 0 ? socketCount : 1;    // Calculate the % per socket power modifier
            MismatchFailChance *= baseMismatchPenaltyReduction;         // Penalty reduction for sockets

            float socketFailChanceSum = 0f;
            for(int i = 0; i < 5; i++)                                      // Socket check
            {
                var socket  = groupFlags[i];
                var circuit = properties[i];
                var delta   = powerDeltas[i];
                if (socket == 0 && circuit == 0)                            // No circuit data in that slot
                    continue;

                var isMatching = CraftingCircuitPropertyMap.Properties[socket].Contains(circuit);

                if (isMatching)
                {
                    socketFailChanceSum += delta * baseFailChance;
                }
                else
                {
                    socketFailChanceSum += (MismatchPenalty / 2);           // We divide by 2 because the other half is below the fail chance
                    socketFailChanceSum += delta * MismatchFailChance;
                }
            }
            var failChanceReduction = GetModifierValue(CraftingModifierType.UnbuffedFailCap, currentCraftInfo.TradeskillType);
            var totalFailChance = socketFailChanceSum *= failChanceReduction;
            return totalFailChance;
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

        public void BeginCraft(uint schematicId)
        {
            var schematic = GameTableManager.Instance.TradeskillSchematic2.GetEntry(schematicId);
            switch (schematic.TradeSkillId)
            {
                case (uint)TradeskillType.Cooking:
                    throw new NotImplementedException();
                case (uint)TradeskillType.Architect:
                    throw new NotImplementedException();
                default:
                    BeginCircuitBoardCrafting(schematic);
                    break;
            }
        }
        private void BeginCircuitBoardCrafting(TradeskillSchematic2Entry entry)
        {
            var type = (TradeskillType)entry.TradeSkillId;
            var modifiers = tradeskills[type].GetModifiers();
            var item = GameTableManager.Instance.Item.GetEntry(entry.Item2IdOutput);
            var itemStats = GameTableManager.Instance.ItemStat.GetEntry(item.ItemStatId);

            //Making a list of sockets that could be used
            var viableSockets = Enum.GetValues<CraftingCircuitSocketType>().ToList();

            //not sure how they appear. Removing for now.
            viableSockets.Remove(CraftingCircuitSocketType.Fusion);

            //Talent Modified socket type removal
            foreach (var socketId in modifiers
                .Where(m => m.ModifierType == CraftingModifierType.SocketRemoval)
                .Select(m => m.ObjectIdSecondary))
            {
                viableSockets.Remove((CraftingCircuitSocketType)socketId);
            }
            //we select all possible properties fitting remaining sockets
            var viableProperties = viableSockets
                .SelectMany(k => CraftingCircuitPropertyMap.Properties[k])
                .ToList();

            //We randomly select properties for each stat slot that is 4 - Craftable
            Random rand = new();
            Property[] selectedProperties       = [0,0,0,0,0];
            CraftingCircuitSocketType[] flags   = [0,0,0,0,0];
            for(int i = 0; i < 5; i++)
            {
                if (itemStats.ItemStatTypeEnum[i] == ItemStatType.Craftable)
                {
                    //selecting a random index
                    //setting a stat at index to that random stat
                    //removing it from a list of available stats (no repetitions)
                    //findign a socket that this property belongs to
                    var rndIdx = rand.Next(viableProperties.Count);
                    selectedProperties[i] = viableProperties[rndIdx];
                    viableProperties.RemoveAt(rndIdx);

                    //this might change when we add fusion sockets since we'll have 2 available sockets for any stat
                    flags[i] = viableSockets.First(s => CraftingCircuitPropertyMap.Properties[s].Contains(selectedProperties[i]));
                }
            }

            //TODO: take materials from the player

            var message = new ServerCraftingCurrentCraft()
            {
                TradeskillSchematic2Id = entry.Id,
                Stats = new CraftStats()
                {
                    StatType = selectedProperties,
                    ApSpSplit = 1,
                    Unknown1 = 30,
                    Unknown2 = 0
                },
                Unused = 1,
                CraftingGroupFlags = flags,
                Item2Id = item.Id,
                SchematicCount = 1
            };
            player.Session.EnqueueMessageEncrypted(message);
            player.Session.EnqueueMessageEncrypted(getModifiersMessage());
        }

        public TradeskillTalentTierEntry GetHighestTalentTier(TradeskillType type)
        {
            var tier = tradeskills[type].GetHighestTalentTier();
            var tradeskillTalentTiers = GameTableManager.Instance.TradeskillTalentTier.Entries
                .Where(t => (TradeskillType)t.TradeSkillId == type)
                .OrderBy(t => t.PointsToUnlock)
                .ToList();
            return tradeskillTalentTiers[(int)tier];
        }

        public void AddGlobalModifier(TradeskillModifierInfo modifier)
        {
            globalModifiers.Add(modifier);
        }

        public void RemoveGlobalModifier(TradeskillModifierInfo modifier)
        {
            globalModifiers.Remove(modifier);
        }

        public void ClearGlobalModifiers()
        {
            globalModifiers.Clear();
        }

        public List<TradeskillModifierInfo> GetModifiers(TradeskillType tradeskill = 0, CraftingModifierType type = 0)
        {
            List<TradeskillModifierInfo> mods = globalModifiers
                .Where(m=> tradeskill == 0 || m.TradeskillAffected == tradeskill)
                .Where(m => type == 0 || m.ModifierType == type)
                .ToList();

            if(tradeskill != 0)
            {
                var tradeskillModifiers = tradeskills[tradeskill].GetModifiers()
                    .Where(m => type == 0 || m.ModifierType == type);
                mods.AddRange(tradeskillModifiers);
            }
            return mods;
        }

        public float GetModifierValue(CraftingModifierType type, TradeskillType tradeskill = 0)
        {
            var modifiers = GetModifiers(tradeskill,type);
            float multiplier = 1f;
            switch (type)
            {
                case CraftingModifierType.MismatchPenalty:
                    return 1 - modifiers.Sum(m => m.ValueFloat) / 100;
                case CraftingModifierType.UnbuffedFailCapBase:
                    return 1 - modifiers.Sum(m => m.ValueFloat) / 100;
                case CraftingModifierType.Charge:
                    return 1 - modifiers.Sum(m => m.ValueFloat) / 100;
                case CraftingModifierType.MaterialCost:
                    return modifiers.Sum(m => m.ValueFloat);
                case CraftingModifierType.Material2Id:
                    return modifiers.Sum(m => m.ValueFloat);
                case CraftingModifierType.UnbuffedFailCap:
                    return 1 - modifiers.Sum(m => m.ValueFloat) / 100;
                case CraftingModifierType.ChargeIncrement_RightShift:
                    return 4 * (1 + modifiers.Sum(m => m.ValueFloat));
                case CraftingModifierType.OutputCount:;
                    foreach(var i in modifiers)
                        multiplier *= i.ValueFloat;
                    return multiplier;
                case CraftingModifierType.IngredientReturn:
                    return 1 + modifiers.Sum(m => m.ValueFloat) / 100;
                case CraftingModifierType.SocketRemoval:
                    return modifiers.Count; //special case where we need to check each modifier
                case CraftingModifierType.AdditiveCost:
                    foreach (var i in modifiers)
                        multiplier *= i.ValueFloat;
                    return multiplier;
                case CraftingModifierType.AdditiveVector:
                    foreach (var i in modifiers)
                        multiplier *= i.ValueFloat;
                    return multiplier;
                case CraftingModifierType.AdditiveRadius:
                    foreach (var i in modifiers)
                        multiplier *= i.ValueFloat;
                    return multiplier;
                case CraftingModifierType.SchematicDiscoveryRadius1:
                    foreach (var i in modifiers)
                        multiplier *= i.ValueFloat;
                    return multiplier;
                case CraftingModifierType.SchematicDiscoveryRadius2:
                    foreach (var i in modifiers)
                        multiplier *= i.ValueFloat;
                    return multiplier;
                case CraftingModifierType.ApSpSplitMaxDelta_LeftShift:
                    return 2 / (1 + modifiers.Sum(m => m.ValueFloat));
                case CraftingModifierType.Cost:
                    foreach (var i in modifiers)
                        multiplier *= i.ValueFloat;
                    return multiplier;
                case CraftingModifierType.CraftIsCritical:
                    return (modifiers.Count > 0? 1 : 0);
                case CraftingModifierType.AdditiveLimit:
                    return modifiers.Sum(m => m.ValueFloat);
                case CraftingModifierType.AdditiveTier:
                    return modifiers.Sum(m => m.ValueFloat);
            }
            return 0;
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
            tradeskills[TradeskillType.Weaponsmith].TalentPoints = 30;


            //debug - no clue what's proficiency about
            //could be linked to tiers?
            //tradeskills[TradeskillType.Cooking].PropertyProficiencyFlags = 35;
        }
    }
}
