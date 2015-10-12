using Autofac;
using CardMaker.Autofac;
using CardMaker.Interfaces;

namespace CardMaker
{
    class Program
    {
        static void Main(string[] args) {
            var builder = new ContainerBuilder();
            builder.RegisterModule<CardMakerModule>();
            //log4net.Config.XmlConfigurator.Configure();
            var container = builder.Build();

            using (var scope = container.BeginLifetimeScope())
            {
                var maker = scope.Resolve<IMakeCards>();
                //maker.SampleCard();
                maker.MakeCards();
            }
        }
    }
}
