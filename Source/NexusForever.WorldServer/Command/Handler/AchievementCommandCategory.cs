using NexusForever.Game.Abstract.Achievement;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Achievement;
using NexusForever.Game.Social;
using NexusForever.Game.Static.Achievement;
using NexusForever.Game.Static.RBAC;
using NexusForever.Game.Static.Social;
using NexusForever.Game.Text.Search;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;
using NexusForever.WorldServer.Command.Context;
using NexusForever.WorldServer.Command.Convert;
using NexusForever.WorldServer.Command.Static;
using System.Collections.Generic;
using System.Linq;

namespace NexusForever.WorldServer.Command.Handler
{
    [Command(Permission.Achievement, "A collection of commands to manage player achievements.", "achievement")]
    [CommandTarget(typeof(IPlayer))]
    public class AchievementCommandCategory : CommandCategory
    {
        [Command(Permission.AchievementUpdate, "Update achievement criteria for player.", "update")]
        public void HandleAchievementUpdate(ICommandContext context,
            [Parameter("Achievement criteria type to update.", ParameterFlags.None, typeof(EnumParameterConverter<AchievementType>))]
            AchievementType type,
            [Parameter("Object id to match against.")]
            uint objectId,
            [Parameter("Alternative object id to match against.")]
            uint objectIdAlt,
            [Parameter("Update count for matched criteria.")]
            uint count)
        {
            IPlayer player = context.GetTargetOrInvoker<IPlayer>();
            player.AchievementManager.CheckAchievements(player, type, objectId, objectIdAlt, count);
        }

        [Command(Permission.AchievementGrant, "Grant achievement to player.", "grant")]
        public void HandleAchievementGrant(ICommandContext context,
            [Parameter("Achievement id to grant.")]
            ushort achievementId)
        {
            IAchievementInfo info = GlobalAchievementManager.Instance.GetAchievement(achievementId);
            if (info == null)
            {
                context.SendMessage($"Invalid achievement id {achievementId}!");
                return;
            }

            context.GetTargetOrInvoker<IPlayer>().AchievementManager.GrantAchievement(achievementId);
        }
        [Command(Permission.AchievementLookup, "Lookup an achievement by partial name.", "lookup")]
        public void HandleAcheivementLookup(ICommandContext context,
            [Parameter("Achievement name to lookup.")]
            string name,
            [Parameter("Maximum amount of results to return.")]
            int? maxResults)
        {
            List<AchievementEntry> searchResults = SearchManager.Instance
                .Search<AchievementEntry>(name, context.Language, e => e.LocalizedTextIdTitle, true)
                .Take(maxResults ?? 25)
                .ToList();

            if (searchResults.Count == 0)
            {
                context.SendMessage($"Achievement lookup results was 0 entries for '{name}'.");
                return;
            }

            context.SendMessage($"Achievement lookup results for '{name}' ({searchResults.Count}):");

            var target = context.GetTargetOrInvoker<IPlayer>();
            foreach (AchievementEntry itemEntry in searchResults)
            {
                var builder = new ChatMessageBuilder
                {
                    Type = ChatChannelType.System,
                    Text = $"({itemEntry.Id}) "
                };
                builder.AppendText(GameTableManager.Instance.TextEnglish.GetEntry(itemEntry.LocalizedTextIdTitle));
                if (itemEntry.AchievementGroupId != 0u)
                {
                    builder.AppendText($" [{itemEntry.AchievementGroupId}]");
                    builder.AppendText($" - {GameTableManager.Instance.AchievementGroup.GetEntry(itemEntry.AchievementGroupId).TradeSkillId}");
                }
                target.Session.EnqueueMessageEncrypted(builder.Build());
            }
        }
    }
}
