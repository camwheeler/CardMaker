using GraphicsMagick;
using System.Collections.Generic;

namespace CardMaker.Objects {
    public class Deck {
        public string Type { get; set; }
        public string Background { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public Dictionary<string, Coordinate> Elements { get; set; }
        public List<Card> Cards { get; set; }
    }
}