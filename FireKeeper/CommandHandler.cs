using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Reflection;
using System.Threading.Tasks;

public class CommandHandler
{
    private CommandService _cmds;
    private DiscordSocketClient _client;
    public static SocketGuildChannel greetingChannel;
    public async Task Install(DiscordSocketClient c)
    {
        _client = c;
        _cmds = new CommandService();

        await _cmds.AddModulesAsync(Assembly.GetEntryAssembly(), null);

        _client.MessageReceived += HandleCommand;


        _client.UserJoined += AnnounceUserJoined;
        _client.UserLeft += AnnounceUserLeft;
    }

    public async Task AnnounceUserJoined(SocketGuildUser user)
    {
        var channel = _client.GetChannel(user.Guild.SystemChannel.Id) as SocketTextChannel;
        await channel.SendMessageAsync($"Welcome {user.Mention} to {channel.Guild.Name}.");
        // await (user as IGuildUser).AddRolesAsync();
    }


    public async Task AnnounceUserLeft(SocketGuildUser user)
    {
        await Task.Delay(0);
    }
    public void code()
    {

    }
    public async Task HandleCommand(SocketMessage s)
    {


        var msg = s as SocketUserMessage;
        string message = msg.ToString();
        if (msg == null) return;
        /* else if (message.ToLower().Contains("nigga"))
        {
            await s.DeleteAsync();
            await s.Channel.SendMessageAsync("You're not allowed to say that.");
        } */

        var context = new CommandContext(_client, msg);

        int argPos = 0;
        string prefix = "-";
        if (msg.HasStringPrefix(prefix, ref argPos))
        {


            var result = await _cmds.ExecuteAsync(context, argPos, null);

            if (!result.IsSuccess)
            {
                switch (result.ToString())
                {
                    default:

                        await s.Channel.SendMessageAsync($"An error occurred! Details: ```" + result.ToString() + "```");
                        break;
                    case "UnknownCommand: Unknown command.":

                        await msg.DeleteAsync();

                        await s.Channel.SendMessageAsync($"Command not found! Use the command {prefix}help for a list of commands.");
                        break;

                }
            }
        }
    }
}
