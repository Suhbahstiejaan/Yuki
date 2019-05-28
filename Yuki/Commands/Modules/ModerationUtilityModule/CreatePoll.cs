using Discord;
using Discord.WebSocket;
using InteractivityAddon;
using Qmmands;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Yuki.Commands.Preconditions;
using Yuki.Data.Objects;
using Yuki.Extensions;
using Yuki.Services;

namespace Yuki.Commands.Modules.ModerationUtilityModule
{
    public partial class ModerationUtilityModule
    {
        [Command("createpoll")]
        [RequireUserPermission(GuildPermission.ManageMessages)]
        public async Task CreatePollAsync(string pollName = "")
        {
            string[] items = new string[] { };
            DateTime deadline = default;
            bool? showVotes = null;

            EmbedBuilder builder = Context.CreateEmbedBuilder(
                    Language.GetString("poll_creating_title_str") + $": {pollName}\n" +
                    Language.GetString("poll_creating_items_str") + $": {Language.GetString("poll_create_items_desc")}\n" +
                    Language.GetString("poll_creating_deadline_str") + $": {Language.GetString("poll_create_deadline_desc")}\n" +
                    Language.GetString("poll_create_show_vote_str") + $": {Language.GetString("poll_create_show_vote_desc")}\n")
                .WithTitle(Language.GetString("poll_create_creating"));

            IUserMessage message = await builder.SendToAsync(Context.Channel);
            InteractivityResult<SocketMessage> result;

            if (string.IsNullOrEmpty(pollName))
            {
                IUserMessage _msg = await ReplyAsync(Language.GetString("poll_create_title"));
                result = await Interactivity.NextMessageAsync(msg => msg.Author == Context.User && msg.Channel == Context.Channel);

                if (result.IsSuccess)
                {
                    pollName = result.Value.Content;

                    await Context.TryDeleteAsync(result.Value);
                    await Context.TryDeleteAsync(_msg);
                }
                else
                {
                    return;
                }
            }

            if (pollName.Length > 300)
            {
                await ReplyAsync(Language.GetString("poll_create_title_long"));
                return;
            }

            await message.ModifyAsync(emb =>
            {
                builder = Context.CreateEmbedBuilder(
                    Language.GetString("poll_creating_title_str") + $": {pollName}\n" +
                    Language.GetString("poll_creating_items_str") + $": {Language.GetString("poll_create_items_desc")}\n" +
                    Language.GetString("poll_creating_deadline_str") + $": {Language.GetString("poll_create_deadline_desc")}\n" +
                    Language.GetString("poll_create_show_vote_str") + $": {Language.GetString("poll_create_show_vote_desc")}\n")
                .WithTitle(Language.GetString("poll_create_creating"));

                emb.Embed = builder.Build();
            });

            while (items.Length < 2)
            {
                IUserMessage _msg = await ReplyAsync(Language.GetString("poll_create_items"));
                result = await Interactivity.NextMessageAsync(msg => msg.Author == Context.User && msg.Channel == Context.Channel);

                if (result.IsSuccess)
                {
                    items = Regex.Split(result.Value.Content, @"\s*[|]\s*");

                    if (items.Length < 2)
                    {
                        await ReplyAsync(Language.GetString("poll_create_items_short"));
                    }

                    await Context.TryDeleteAsync(result.Value);
                    await Context.TryDeleteAsync(_msg);
                }
                else
                {
                    return;
                }
            }

            await message.ModifyAsync(emb =>
            {
                builder = Context.CreateEmbedBuilder(
                    Language.GetString("poll_creating_title_str") + $": {pollName}\n" +
                    Language.GetString("poll_creating_items_str") + $": {string.Join(", ", items)}\n" +
                    Language.GetString("poll_creating_deadline_str") + $": {Language.GetString("poll_create_deadline_desc")}\n" +
                    Language.GetString("poll_create_show_vote_str") + $": {Language.GetString("poll_create_show_vote_desc")}\n")
                .WithTitle(Language.GetString("poll_create_creating"));

                emb.Embed = builder.Build();
            });

            while (deadline == default)
            {
                IUserMessage _msg = await ReplyAsync(Language.GetString("poll_create_deadline"));
                result = await Interactivity.NextMessageAsync(msg => msg.Author == Context.User && msg.Channel == Context.Channel);

                if (result.IsSuccess)
                {
                    deadline = result.Value.Content.ToDateTime();

                    if (deadline == default)
                    {
                        await ReplyAsync(Language.GetString("poll_create_deadline_invalid"));
                    }
                    else if ((deadline.TimeOfDay.TotalDays / 7) > 2)
                    {
                        await ReplyAsync(Language.GetString("poll_create_deadline_long"));
                    }

                    await Context.TryDeleteAsync(result.Value);
                    await Context.TryDeleteAsync(_msg);
                }
                else
                {
                    return;
                }
            }

            await message.ModifyAsync(emb =>
            {
                builder = Context.CreateEmbedBuilder(
                    Language.GetString("poll_creating_title_str") + $": {pollName}\n" +
                    Language.GetString("poll_creating_items_str") + $": {string.Join(", ", items)}\n" +
                    Language.GetString("poll_creating_deadline_str") + $": {deadline.ToPrettyTime(false)}\n" +
                    Language.GetString("poll_create_show_vote_str") + $": {Language.GetString("poll_create_show_vote_desc")}\n")
                .WithTitle(Language.GetString("poll_create_creating"));

                emb.Embed = builder.Build();
            });

            while (showVotes == null)
            {
                IUserMessage _msg = await ReplyAsync(Language.GetString("poll_create_allow_view"));
                result = await Interactivity.NextMessageAsync(msg => msg.Author == Context.User && msg.Channel == Context.Channel);

                if (result.IsSuccess)
                {
                    if (result.Value.Content.ToLower() == "y" || result.Value.Content == "n")
                    {
                        showVotes = result.Value.Content.ToLower() == "y";
                    }

                    await Context.TryDeleteAsync(result.Value);
                    await Context.TryDeleteAsync(_msg);
                }
                else
                {
                    return;
                }
            }

            if (showVotes is bool ShowVotes)
            {
                Poll poll = Poll.Create(pollName, items, Context.Guild.Id, deadline, ShowVotes);
                PollingService.Save(poll);

                await message.ModifyAsync(emb =>
                {
                    builder = Context.CreateEmbedBuilder(
                        Language.GetString("poll_creating_title_str") + $": {pollName}\n" +
                        Language.GetString("poll_creating_items_str") + $": {string.Join(", ", items)}\n" +
                        Language.GetString("poll_creating_deadline_str") + $": {deadline.ToPrettyTime(false)}\n" +
                        Language.GetString("poll_create_show_vote_str") + $": {showVotes}\n")
                    .WithTitle(Language.GetString("poll_create_created"))
                    .WithFooter(Language.GetString("poll_created_id") + $": {poll.Id}");

                    emb.Embed = builder.Build();
                });

                await ReplyAsync(Language.GetString("poll_create_created") + $" ID: {poll.Id}");
            }
            else
            {
                await ReplyAsync("Uh oh! Something bad happened!");
            }
        }
    }
}