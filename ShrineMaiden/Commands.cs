using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FireKeeper
{
    public class Commands : ModuleBase
    {
        [Command("help"), Alias("h")]
        public async Task Help()
        {
            EmbedBuilder embed = new EmbedBuilder();

            embed.WithTitle("Fire Keeper Help Menu");
            embed.WithDescription("The Fire Keeper is a simple discord bot that will slowly grow the more people use it");
            embed.AddField("Commands", "**8Ball** - Ask a question, get an answer. `#8ball <question>`\n" +
                                       "**PFP** - Clearer look at a profile picture `#pfp @someone` or your own picture just `#pfp`\n" +
                                       "**adminHelp** - A special help menu with admin commands.\n");
            embed.WithColor(Color.DarkRed);

            await ReplyAsync("", false, embed.Build());
        }

        [Command("adminhelp"), Alias("ah")]
        public async Task AdminHelp()
        {
            EmbedBuilder embed = new EmbedBuilder();

            embed.WithTitle("Fire Keeper Admin Help Menu");
            embed.WithDescription("These commands require certain privileges to use");
            embed.AddField("Commands", "**role** - Gives someone a role.\n Takes away the role if user already has it.\n `#role @role @user : Requires Permission 'Manage Roles'`\n" +
                                       "**kick** - Kicks someone out and displays a reason if one is given.\n `#kick @user \"reason\" : Requires Permission 'Kick Members'`\n");
            embed.WithColor(Color.DarkRed);

            await ReplyAsync("", false, embed.Build());
        }

        // Admin Commands

        [Command("role")]
        public async Task Role(SocketRole role, SocketGuildUser mention = null)
        {
            var user = Context.User as SocketGuildUser;
            if (!user.GuildPermissions.ManageRoles) await ReplyAsync("You're not allowed to do that.");
            else if (mention == null) await ReplyAsync($"Choose someone to get the {role} role");
            else if(mention.Roles.Contains(role)) { await (mention as IGuildUser).RemoveRoleAsync(role); }
            else{ await (mention as IGuildUser).AddRoleAsync(role); }
        }

        [Command("kick")]
        public async Task Kick(SocketGuildUser mention = null, string reason = null)
        {
            var user = Context.User as SocketGuildUser;
            if (mention == null) { await ReplyAsync("Tell me who you want to kick."); }
            else if (user == mention) { await ReplyAsync("You can't kick yourself."); }
            else if (mention.IsBot) { await ReplyAsync("You are unable to do that."); }
            else if (mention != null && user.GuildPermissions.KickMembers)
            {
                var channel = await mention.GetOrCreateDMChannelAsync();
                await channel.SendMessageAsync(reason == null ? $"You have been kicked" : $"You have been kicked for the reason: {reason}");

                await Task.Delay(2000);
                await mention.KickAsync();

                await ReplyAsync(reason == null ? $"I have gotten rid of {mention.Username}" : $"I have gotten rid of {mention.Username}.\n Reason: {reason}");
            }
            else { await ReplyAsync("You're not allowed to kick people"); }
        }
        
        [Command("setjoinchannel")]
        public async Task setJoinChannel(SocketGuildChannel joinChannel)
        {
            CommandHandler.greetingChannel = joinChannel;
        }

        [Command("test")]
        public async Task test()
        {
            var msg = Context.Message as SocketUserMessage;
            EmbedBuilder embed = new EmbedBuilder();
            embed.WithTitle("You sent this picture:");
            embed.WithImageUrl(msg.Attachments.FirstOrDefault().Url);

            await ReplyAsync("", false, embed.Build());
        }

        // Commands for everyone

        [Command("8ball")]
        public async Task _8Ball([Remainder]string question)
        {
            string[] responses = { "Yes", "No", "Not sure", "Ask again later" };
            var answer = responses[new Random().Next(responses.Count())];
            EmbedBuilder embed = new EmbedBuilder();

            embed.AddField("8 ball", $"{(Context.User as SocketGuildUser).Username},");
            embed.AddField("Question", $"You asked: **[{question}]**");
            embed.AddField("Answer", $"Your answer is **[{answer}]**");

            switch (answer)
            {
                case "Yes":
                    {
                        embed.WithColor(new Color(0, 255, 0));
                        break;
                    }
                case "No":
                    {
                        embed.WithColor(new Color(255, 0, 0));
                        break;
                    }
                case "Not sure":
                    {
                        embed.WithColor(new Color(255, 255, 0));
                        break;
                    }
                case "Ask again later":
                    {
                        embed.WithColor(new Color(255, 0, 255));
                        break;
                    }
            }

            await ReplyAsync("", false, embed.Build());
        }

        [Command("pfp")]
        public async Task pfp(SocketGuildUser mention = null)
        {
            EmbedBuilder Embed = new EmbedBuilder();
            // Info of the user calling the command
            var author = Context.User as SocketGuildUser;
            string name = author.Nickname == null ? author.Username : author.Nickname;
            Embed.WithAuthor(name, author.GetAvatarUrl()); 

            // Info of the user being displayed
            var user = mention == null ? Context.User as SocketGuildUser : mention;
            var username = user.Nickname == null ? user.Username : user.Nickname;
            Embed.Title = username + "'s profile picture";
            Embed.ImageUrl = user.GetAvatarUrl();

            if (author.Roles.Count > 1)   // Gives the embed a color that matches the user's top role 
            {
                bool skip = true;
                foreach (SocketRole role in ((SocketGuildUser)Context.Message.Author).Roles)
                {
                    if (skip) { skip = false; continue; }  // Skips the invisible @everyone role
                    Embed.WithColor(role.Color); break;
                }
            }

            // Displays everything
            await ReplyAsync("", false, Embed.Build());
        }
    }
}
