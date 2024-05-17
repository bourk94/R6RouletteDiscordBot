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
            try
            {
                porcupine = Porcupine.FromBuiltInKeywords(
                token,
                new List<BuiltInKeyword> { BuiltInKeyword.JARVIS });
            }
            catch (Pv.PorcupineActivationLimitException ex)
            {
                Console.WriteLine("Erreur lors de l'initialisation de Porcupine : " + ex.Message);
                Console.WriteLine("Veuillez vérifier que votre clé d'accès est correcte et que vous n'avez pas dépassé votre limite d'activation.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Une erreur inattendue s'est produite : " + ex.Message);
            }
        }

        public async Task ReceiveHandler(VoiceNextConnection _, VoiceReceiveEventArgs args)
        {

            byte[] pcmData = args.PcmData.ToArray();

            // Convertir byte[] en short[]
            short[] audioFrame = new short[pcmData.Length / sizeof(short)];
            Buffer.BlockCopy(pcmData, 0, audioFrame, 0, pcmData.Length);

            if (audioFrame.Length > porcupine.FrameLength)
            {
                int frameSize = porcupine.FrameLength;
                int offset = audioFrame.Length - porcupine.FrameLength;

                for (int i = 0; i + frameSize <= audioFrame.Length; i += offset)
                {
                    short[] frame = new short[frameSize];
                    Array.Copy(audioFrame, i, frame, 0, frameSize);

                    // Passer la trame à Porcupine
                    if (porcupine != null)
                    {
                        int keywordIndex = porcupine.Process(frame);
                        if (keywordIndex >= 0)
                        {
                            await commandRoulette.RouletteStrat(GetCommandContext());
                        }
                    }
                    else
                    {
                        Console.WriteLine("Porcupine n'est pas initialisé");
                    }
                }
            }
            else
            {
                // Traiter les données audio par trames
                for (int i = 0; i < audioFrame.Length; i += porcupine.FrameLength)
                {
                    short[] frame = new short[porcupine.FrameLength];

                    // Copier les données audio dans la trame
                    int remaining = audioFrame.Length - i;
                    Array.Copy(audioFrame, i, frame, 0, Math.Min(remaining, porcupine.FrameLength));

                    // Si nous n'avons pas assez de données audio pour une trame complète, compléter avec des zéros
                    if (remaining < porcupine.FrameLength)
                    {
                        for (int j = remaining; j < porcupine.FrameLength; j++)
                        {
                            frame[j] = 0;
                        }
                    }

                    // Passer la trame à Porcupine
                    if (porcupine != null)
                    {
                        int keywordIndex = porcupine.Process(frame);
                        if (keywordIndex >= 0)
                        {
                            await commandRoulette.RouletteStrat(GetCommandContext());
                        }
                    }
                    else
                    {
                        Console.WriteLine("Porcupine n'est pas initialisé");
                    }
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
