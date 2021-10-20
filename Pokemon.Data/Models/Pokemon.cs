using System.Collections.Generic;

namespace Pokemon.Data.Models
{
    public class Pokemon
    {
        public string Name { get; set; }

        public List<FlavorText> Flavor_Text_Entries { get; set; }

        public Habitat Habitat { get; set; }

        public bool IsLegendary { get; set; }
    }

    public class Habitat
    {
        public string Name { get; set; }
    }

    public class FlavorText
    { 
        public string Flavor_Text { get; set; }

        public Language Language { get; set; }
    }

    public class Language
    {
        public string Name { get; set; }
    }
}
