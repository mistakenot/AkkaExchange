using System;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.DI.AutoFac;
using Akka.DI.Core;
using AkkaExchange.Client.Actors;
using AkkaExchange.Client.Commands;
using Autofac;

namespace AkkaExchange
{
    public class AkkaExchange : IDisposable
    {
        public ActorSystem System { get; }
        
        private readonly IActorRef _clientManager;
        private readonly AutoFacDependencyResolver _resolver;

        public AkkaExchange(IContainer container)
        {
            System = ActorSystem.Create("akka-exchange-system");

            var resolver = new AutoFacDependencyResolver(container, System);

            System.AddDependencyResolver(resolver);
            
            _clientManager = System.ActorOf(
                System.DI().Props<ClientManagerActor>(), "client-manager");
        }

        public void Dispose()
        {
            System.Dispose();
        }

        public async Task<AkkaExchangeClient> NewConnection()
        {
            var command = new StartConnectionCommand();
            var responseObject = await _clientManager.Ask(command);
            var inbox = Inbox.Create(System);

            return new AkkaExchangeClient(
                Guid.NewGuid(),
                responseObject as IActorRef,
                inbox);
        }

        public async Task EndConnection(Guid connectionId)
        {
            var command = new EndConnectionCommand(connectionId);
            var responseObject = await _clientManager.Ask(command);
        }
    }
}