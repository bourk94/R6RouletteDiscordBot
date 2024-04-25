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

            using (var reader = new StreamReader(Path.Combine(projectDirectory, "List_R6_Roulette", "AttackList.xml")))
            {
                Attack = (BdDefi)XmlSerializer.Deserialize(reader);
            }

            using (var reader = new StreamReader(Path.Combine(projectDirectory, "List_R6_Roulette", "DefenceList.xml")))
            {
                Defence = (BdDefi)XmlSerializer.Deserialize(reader);
            }

            using (var reader = new StreamReader(Path.Combine(projectDirectory, "List_R6_Roulette", "PenalityList.xml")))
            {
                Penality = (BdDefi)XmlSerializer.Deserialize(reader);
            }

        }
    }
}