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
        private readonly CommandRoulette commandRoulette;
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
            short[] frame = new short[porcupine.FrameLength];

            byte[] pcmData = args.PcmData.ToArray();

            short[] audioFrame = new short[pcmData.Length / sizeof(short)];
            Buffer.BlockCopy(pcmData, 0, audioFrame, 0, pcmData.Length);

            if (audioFrame.Length > porcupine.FrameLength)
            {
                int offset = audioFrame.Length - porcupine.FrameLength;

                for (int i = 0; i + porcupine.FrameLength <= audioFrame.Length; i += offset)
                {
                    Array.Copy(audioFrame, i, frame, 0, porcupine.FrameLength);
                }
            }
            else
            {
                for (int i = 0; i < audioFrame.Length; i += porcupine.FrameLength)
                {
                    int remaining = audioFrame.Length - i;
                    Array.Copy(audioFrame, i, frame, 0, Math.Min(remaining, porcupine.FrameLength));

                    if (remaining < porcupine.FrameLength)
                    {
                        for (int j = remaining; j < porcupine.FrameLength; j++)
                        {
                            frame[j] = 0;
                        }
                    }
                }
            }

            if (porcupine != null)
            {
                int keywordIndex = porcupine.Process(frame);
                if (keywordIndex >= 0)
                {
                    await commandRoulette.RouletteStrat(commandContext);
                }
            }
            else
            {
                Console.WriteLine("Porcupine n'est pas initialisé");
            }

            await Task.Yield();
        }
    }
}
