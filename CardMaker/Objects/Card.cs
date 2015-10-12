using System.Collections.Generic;

namespace CardMaker.Objects
{
    public class Card
    {
        public string Name { get; set; }
        public Dictionary<string, string> ElementData { get; set; }
    }
}