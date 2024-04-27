using System;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.IO;

namespace R6_Roulette_Bot
{
    class Program
    {
        internal static BdDefi Attack;
        internal static BdDefi Defence;
        internal static BdDefi Penality;
        static void Main(string[] args)
        {
            Attack = new BdDefi();
            Defence = new BdDefi();
            Penality = new BdDefi();

            DeserialixeXmlFileToList();

            Bot unBot = new Bot();
            unBot.RunAsync().GetAwaiter().GetResult();
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