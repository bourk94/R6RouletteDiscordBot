using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using System.IO;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using R6_Roulette_Bot.Commands;
using DSharpPlus.VoiceNext;

// https://discord.com/api/oauth2/authorize?client_id=1028525800758190161&permissions=412317379648&scope=bot
namespace R6_Roulette_Bot
{
    public class Bot
    {
        private string projectDirectory = AppDomain.CurrentDomain.BaseDirectory;
        public DiscordClient? Client { get; private set; }
        public CommandsNextExtension? Commands { get; private set; }
        
        public async Task RunAsync()
        {
            var json = string.Empty;

            ConfigJson configJson;
            try
            {
                using (var fs = File.OpenRead(Path.Combine(projectDirectory, "List_R6_Roulette", "config.json")))
                using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                    json = await sr.ReadToEndAsync().ConfigureAwait(false);

                configJson = JsonConvert.DeserializeObject<ConfigJson>(json);
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine("Le fichier de configuration est manquant : " + ex.Message);
                return;
            }
            catch (JsonException ex)
            {
                Console.WriteLine("Erreur lors de la lecture du fichier de configuration : " + ex.Message);
                return;
            }

            var config = new DiscordConfiguration
            {
                Token = configJson.Token,
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                MinimumLogLevel = LogLevel.Debug,
                LoggerFactory = default,
                Intents = DiscordIntents.All
            };

            Client = new DiscordClient(config);

            Client.UseVoiceNext(
                   new VoiceNextConfiguration
                   {
                       EnableIncoming = true,
                       AudioFormat = new AudioFormat(16000, 1, VoiceApplication.Voice)
                   }
                );

            Client.Ready += OnClientReady;

            var commandsConfig = new CommandsNextConfiguration
            {
                StringPrefixes = new string[] { configJson.Prefix },
                EnableMentionPrefix = true,
                EnableDms = false,
                EnableDefaultHelp = true,
                UseDefaultCommandHandler = true,
                QuotationMarks = new char[] { '"' }
            };
            

            Commands = Client.UseCommandsNext(commandsConfig);

            Commands.RegisterCommands<CommandRoulette>();

            await Client.ConnectAsync();

            await Task.Delay(-1);
        }

        private Task OnClientReady(DiscordClient Client, ReadyEventArgs e)
        {
            return Task.CompletedTask;
        }
    }
}
