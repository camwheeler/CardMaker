using System.Configuration;

namespace CardMaker
{
    public class CardMakerConfig
    {
        public string SavePath {
            get { return ConfigurationManager.AppSettings["SavePath"]; }
        }
    }
}
