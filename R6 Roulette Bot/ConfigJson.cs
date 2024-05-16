using Newtonsoft.Json;

namespace R6_Roulette_Bot
{
    public struct ConfigJson
    {
        [JsonProperty("discordtoken")]
        public string DiscordToken { get; private set; }

        [JsonProperty("picovoicetoken")]
        public string PicovoiceToken { get; private set; }

        [JsonProperty("prefix")]
        public string Prefix { get; private set; }
    }
}
