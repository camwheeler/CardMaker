using Autofac;
using CardMaker.Implementations;
using CardMaker.Interfaces;

namespace CardMaker.Autofac
{
    class CardMakerModule : Module
    {
        protected override void Load(ContainerBuilder builder) {
            builder.RegisterType<CardGenerator>().As<IMakeCards>();
        }
    }
}
