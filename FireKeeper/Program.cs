using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace FireKeeper
{
    class Program
    {
        public DiscordSocketClient Client;
        public CommandHandler Handler;

        static void Main(string[] args) => new Program().Start().GetAwaiter().GetResult();

        private async Task Start()
        {
            Client = new DiscordSocketClient();

            Handler = new CommandHandler();

            await Client.LoginAsync(Discord.TokenType.Bot, "Token", true);

            await Client.StartAsync();

            await Handler.Install(Client);

            Client.Ready += Client_Ready;
            await Task.Delay(-1);
        }

        private async Task Client_Ready()
        {
            Console.WriteLine("Bot ready!");
            return;
        }
    }
}
