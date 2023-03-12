using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace R6_Roulette_Bot
{
    public class Defi
    {
        [XmlAttribute("Name")]
        public string? Name { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is Defi defi &&
                   Name == defi.Name;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
