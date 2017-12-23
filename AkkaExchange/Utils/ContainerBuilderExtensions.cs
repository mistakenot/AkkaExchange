using Autofac;

namespace AkkaExchange.Utils
{
    public static class ContainerBuilderExtensions
    {
        public static ContainerBuilder AddAkkaExchangeDependencies(this ContainerBuilder builder)
        {
            return builder;
        }
    }
}