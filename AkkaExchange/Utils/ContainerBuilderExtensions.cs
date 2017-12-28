using AkkaExchange.Client;
using AkkaExchange.Client.Actors;
using AkkaExchange.Execution.Actors;
using AkkaExchange.Orders;
using AkkaExchange.Orders.Actors;
using Autofac;

namespace AkkaExchange.Utils
{
    public static class ContainerBuilderExtensions
    {
        public static ContainerBuilder AddAkkaExchangeDependencies(this ContainerBuilder builder)
        {
            builder.RegisterType<ClientActor>();
            builder.RegisterType<ClientHandler>().As<ICommandHandler<ClientState>>();

            builder.RegisterType<ClientManagerActor>();
            builder.RegisterType<ClientManagerHandler>().As<ICommandHandler<ClientManagerState>>();

            builder.RegisterType<OrderBookActor>();
            builder.RegisterType<OrderBookHandler>().As<ICommandHandler<OrderBookState>>();
            // builder.RegisterType<DefaultOrderMatcher>().As<IOrderMatcher>();

            builder.RegisterType<OrderExecutorActor>();

            return builder;
        }
    }
}