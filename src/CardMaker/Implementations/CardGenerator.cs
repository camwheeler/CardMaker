using CardMaker.Interfaces;
using CardMaker.Objects;
using GraphicsMagick;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace CardMaker.Implementations {
    public class CardGenerator : IMakeCards {
        public void MakeCards() {
            var deck = JsonConvert.DeserializeObject<Deck>(File.ReadAllText(@"C:\temp\testdeck.cm"));

            var cards = new List<MagickImage>();
            foreach (var card in deck.Cards) {
                var image = new MagickImage(new MagickColor("WhiteSmoke"), deck.Width, deck.Height);
                image.Format = MagickFormat.Bmp;
                foreach (var element in deck.Elements) {
                    var data = card.ElementData[element.Key];
                    if (File.Exists(data)) {
                        using (var overlayImage = new MagickImage(data)) {
                            image.Composite(overlayImage, (int)element.Value.X, (int)element.Value.Y, CompositeOperator.Over);
                        }
                    } else {
                        using (var textImage = new MagickImage(MagickColor.Transparent, deck.Width, deck.Height)) {
                            textImage.Font = "Arial";
                            textImage.FillColor = new MagickColor("Black");
                            textImage.Draw(new DrawableText(element.Value.X, element.Value.Y, data));
                            image.Composite(textImage, CompositeOperator.Over);
                        }
                    }
                }
                image.Write(string.Format(@"c:\temp\CardMaker\{0}.png", card.Name));
                cards.Add(image);

            }
            using (var doc = new Document()) {
                PdfWriter.GetInstance(doc, new FileStream(@"C:\temp\CardMaker\cards.pdf", FileMode.Create));
                doc.Open();
                doc.NewPage();
                var table = new PdfPTable((int)Math.Floor(612d / (deck.Width + 10)));

                table.DefaultCell.Border = 0;
                table.DefaultCell.Padding = 5;
                foreach (var card in cards) {
                    table.AddCell(Image.GetInstance(card.ToByteArray()));
                }
                table.CompleteRow();
                doc.Add(table);
            }
        }

        public void SampleCard() {
            if (!File.Exists(@"C:\temp\testdeck.cm")) {
                var deck = new Deck {
                    Type = "Sample Deck",
                    Width = 216,
                    Height = 108,
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
