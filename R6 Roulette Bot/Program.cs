using Pv;
using System.Xml.Serialization;

namespace R6_Roulette_Bot
{
    class Program
    {
        internal static BdDefi Attack;
        internal static BdDefi Defence;
        internal static BdDefi Penality;

        internal static Porcupine porcupine;

        static void Main(string[] args)
        {
            Attack = new BdDefi();
            Defence = new BdDefi();
            Penality = new BdDefi();

            DeserialixeXmlFileToList();
            InitPorcupine();

            Bot unBot = new Bot();
            unBot.RunAsync().GetAwaiter().GetResult();
        }

        private static void InitPorcupine()
        {
            porcupine = Porcupine.FromBuiltInKeywords(
            "n985H1zNTKSwCECcySy4jn/6jImBWEAcNUO5T2HjmiCAcYeI3Le3gw==",
            new List<BuiltInKeyword> { BuiltInKeyword.PORCUPINE, BuiltInKeyword.JARVIS });
        }

        private static void DeserialixeXmlFileToList()
        {
            var XmlSerializer = new XmlSerializer(typeof(BdDefi));

            string projectDirectory = AppDomain.CurrentDomain.BaseDirectory;

            string attackListPath = Path.Combine(projectDirectory, "List_R6_Roulette", "AttackList.xml");
            if (!File.Exists(attackListPath))
            {
                File.Create(attackListPath).Close();
            }
            if (new FileInfo(attackListPath).Length > 0)
            {
                using (var reader = new StreamReader(attackListPath))
                {
                    Attack = (BdDefi)XmlSerializer.Deserialize(reader);
                }
            }

            string defenceListPath = Path.Combine(projectDirectory, "List_R6_Roulette", "DefenceList.xml");
            if (!File.Exists(defenceListPath))
            {
                File.Create(defenceListPath).Close();
            }
            if (new FileInfo(defenceListPath).Length > 0)
            {
                using (var reader = new StreamReader(defenceListPath))
                {
                    Defence = (BdDefi)XmlSerializer.Deserialize(reader);
                }
            }

            string penalityListPath = Path.Combine(projectDirectory, "List_R6_Roulette", "PenalityList.xml");
            if (!File.Exists(penalityListPath))
            {
                File.Create(penalityListPath).Close();
            }
            if (new FileInfo(penalityListPath).Length > 0)
            {
                using (var reader = new StreamReader(penalityListPath))
                {
                    Penality = (BdDefi)XmlSerializer.Deserialize(reader);
                }
            }
        }
    }
}