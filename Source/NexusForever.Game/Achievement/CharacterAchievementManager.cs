using NexusForever.Database.Character.Model;
using NexusForever.Game.Abstract.Achievement;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Guild;
using NexusForever.Game.Static.Achievement;
using NexusForever.Game.Static.Crafting;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;

namespace NexusForever.Game.Achievement
{
    public sealed class CharacterAchievementManager : BaseAchievementManager<CharacterAchievementModel>, ICharacterAchievementManager
    {
        private readonly IPlayer owner;
        protected override ulong OwnerId => owner.CharacterId;

        /// <summary>
        /// Create a new <see cref="CharacterAchievementManager"/> from existing <see cref="CharacterModel"/> database model.
        /// </summary>
        public CharacterAchievementManager(IPlayer owner, CharacterModel model)
        {
            this.owner = owner;
            Initialise(model.Achievement, true);
        }

        /// <summary>
        /// Send initial <see cref="IAchievement"/> information to owner on login.
        /// </summary>
        /// <remarks>
        /// Guild achievements will also be sent if owner is part of a <see cref="IGuild"/>.
        /// </remarks>
        public override void SendInitialPackets(IPlayer _)
        {
            base.SendInitialPackets(owner);
            owner.GuildManager.Guild?.AchievementManager.SendInitialPackets(owner);
        }

        protected override void SendAchievementUpdate(IEnumerable<IAchievement> updates)
        {
            owner.Session.EnqueueMessageEncrypted(BuildAchievementUpdate(updates));
        }

        /// <summary>
        /// Update or complete player achievements of <see cref="AchievementType"/> as <see cref="IPlayer"/> with supplied object ids.
        /// </summary>
        public override void CheckAchievements(IPlayer target, AchievementType type, uint objectId, uint objectIdAlt = 0u, uint count = 1u)
        {
            CheckAchievements(target, GlobalAchievementManager.Instance.GetCharacterAchievements(type), objectId, objectIdAlt, count);
            target.GuildManager.Guild?.AchievementManager.CheckAchievements(target, type, objectId, objectIdAlt, count);
        }

        protected override void CompleteAchievement(IAchievement achievement)
        {
            base.CompleteAchievement(achievement);

            if (achievement.Info.Entry.CharacterTitleId != 0u)
                owner.TitleManager.AddTitle((ushort)achievement.Info.Entry.CharacterTitleId);

            //Tradeskills
            GrantTradeskillRewards(achievement);
            

            // TODO
            /*if (isRealmFirst)
            {
                MapManager.BroadcastMessage(new ServerRealmFirstAchievement
                {
                    AchievementId = achievement.Id,
                    Player        = owner.Name
                });
            }*/
        }
        private void GrantTradeskillRewards(IAchievement achievement)
        {
            if (achievement.Info.Entry.AchievementGroupId == 0)
                return; //Very likely not a tradeskill achievement because it is not in any UI

            //find category parent
            var categoryParent = GameTableManager.Instance.AchievementCategory.GetEntry(achievement.Info.Entry.AchievementCategoryId).AchievementCategoryIdParent;

            //find tradeskill parenting the category id
            var tradeskillInfo = GameTableManager.Instance.Tradeskill.Entries
                .FirstOrDefault(t => t.AchievementCategoryId == categoryParent, null);

            if (tradeskillInfo == null)
                return;//Does not belong to the tradeskill category

            TradeskillAchievementRewardEntry rewardEntry = GameTableManager.Instance.TradeskillAchievementReward.Entries
                .FirstOrDefault(t => t.AchievementId == achievement.Id, null);

            if (rewardEntry == null)
                return; //no reward asociated
                        
            TradeskillType type = (TradeskillType)tradeskillInfo.Id;

            //Now that we have a tradeskill type and the reward entry we can give the user his rewards.
            owner.TradeskillManager.GrantTalentPoints(type, rewardEntry.TalentPoints);

        }
    }
}
