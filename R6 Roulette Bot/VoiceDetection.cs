using DSharpPlus.CommandsNext;
using DSharpPlus.VoiceNext;
using DSharpPlus.VoiceNext.EventArgs;
using Pv;
using R6_Roulette_Bot.Commands;


namespace R6_Roulette_Bot
{
    internal class VoiceDetection
    {

        public CommandContext? commandContext { get; set; }
        private CommandRoulette commandRoulette;
        private static Porcupine? porcupine;

        public VoiceDetection(CommandRoulette commandRoulette)
        {
            this.commandRoulette = commandRoulette;
        }

        public static void InitPorcupine(string token)
        {
            porcupine = Porcupine.FromBuiltInKeywords(
            token,
            new List<BuiltInKeyword> { BuiltInKeyword.JARVIS });
        }

        public async Task ReceiveHandler(VoiceNextConnection _, VoiceReceiveEventArgs args)
        {

            byte[] pcmData = args.PcmData.ToArray();

            // Convertir byte[] en short[]
            short[] audioFrame = new short[pcmData.Length / sizeof(short)];
            Buffer.BlockCopy(pcmData, 0, audioFrame, 0, pcmData.Length);


            // Traiter les données audio par trames
            for (int i = 0; i < audioFrame.Length; i += 512)
            {
                short[] frame = new short[512];

                // Copier les données audio dans la trame
                int remaining = audioFrame.Length - i;
                Array.Copy(audioFrame, i, frame, 0, Math.Min(remaining, 512));

                // Si nous n'avons pas assez de données audio pour une trame complète, compléter avec des zéros
                if (remaining < 512)
                {
                    for (int j = remaining; j < 512; j++)
                    {
                        frame[j] = 0;
                    }
                }

                // Passer la trame à Porcupine
                int keywordIndex = porcupine.Process(frame);
                if (keywordIndex >= 0)
                {
                    await commandRoulette.RouletteStrat(GetCommandContext());
                }
            }
            await Task.Yield();
        }

        public void SetCommandContext(CommandContext ctx)
        {
            commandContext = ctx;
        }

        private CommandContext GetCommandContext()
        {
            return commandContext;
        }
    }
}
