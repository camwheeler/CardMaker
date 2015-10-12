using System.Collections.Generic;
using System.IO;
using CardMaker.Interfaces;
using CardMaker.Objects;
using GraphicsMagick;
using iTextSharp.text.pdf;
using Newtonsoft.Json;

namespace CardMaker.Implementations
{
    public class CardGenerator : IMakeCards {
        public void MakeCards() {
            var deck = JsonConvert.DeserializeObject<Deck>(File.ReadAllText(@"C:\temp\testdeck.cm"));

            var cards = new List<MagickImage>();
            foreach (var card in deck.Cards) {
                using (var image = new MagickImage(new MagickColor(deck.Background), deck.Width, deck.Height)) {
                    image.Format = MagickFormat.Png;
                    foreach (var element in deck.Elements) {
                        var data = card.ElementData[element.Key];
                        if (File.Exists(data)) {
                            using (var overlayImage = new MagickImage(data)) {
                                image.Composite(overlayImage, (int)element.Value.X, (int)element.Value.Y, CompositeOperator.Over);
                            }
                        } else {
                            using (var textImage = new MagickImage(MagickColor.Transparent, deck.Width, deck.Height)) {
                                textImage.Font = "Arial";
                                textImage.Draw(new DrawableText(element.Value.X, element.Value.Y, data));
                                image.Composite(textImage, CompositeOperator.Overlay);
                            }
                        }
                    }
                    image.Write(string.Format(@"c:\temp\CardMaker\{0}.png", card.Name));
                    cards.Add(image);
                }
            }
            var table = new PdfPTable(3);
            foreach (var card in cards) {
                table.AddCell(card)
            }
        }

        public void SampleCard() {
            if (!File.Exists(@"C:\temp\testdeck.cm")) {
                var deck = new Deck {
                    Type = "Sample Deck",
                    Width = 200,
                    Height = 200,
                    Background = "Green",
                    Elements = new Dictionary<string, Coordinate> {
                        {"Title", new Coordinate(52, 80)},
                        {"Picture", new Coordinate(13, 18)},
                        {"HP", new Coordinate(20, 46)},
                    },
                    Cards = new List<Card> {
                        new Card {
                            Name = "A Sample Card",
                            ElementData = new Dictionary<string, string> {
                                {"Title", "Sample Card"},
                                {"Picture", @"c:\temp\CardMaker\face.png"},
                                {"HP", "HP: 32"},
                            }
                        }
                    }
                };
                File.WriteAllText(@"C:\temp\testdeck.cm", JsonConvert.SerializeObject(deck));
            }
        }
    }
}