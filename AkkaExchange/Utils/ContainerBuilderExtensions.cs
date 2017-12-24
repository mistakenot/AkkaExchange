using AkkaExchange.Client;
using AkkaExchange.Client.Actors;
using AkkaExchange.Client.Commands;
using Autofac;

namespace AkkaExchange.Utils
{
    public static class ContainerBuilderExtensions
    {
        public static ContainerBuilder AddAkkaExchangeDependencies(this ContainerBuilder builder)
        {
            builder.RegisterType<ClientActor>();
            builder.RegisterType<ClientActorStartConnectionCommandHandler>().As<ICommandHandler<ClientState>>();
            builder.RegisterType<ClientManagerActor>();
            builder.RegisterType<ClientManagerCommandHandler>().As<ICommandHandler<ClientManagerState>>();

            return builder;
        }
    }
}