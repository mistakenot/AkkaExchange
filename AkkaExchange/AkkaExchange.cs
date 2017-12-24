using System;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.DI.Core;
using AkkaExchange.Client.Actors;
using AkkaExchange.Client.Commands;
using AkkaExchange.Client.Events;

namespace AkkaExchange
{
    public class AkkaExchange : IDisposable
    {
        public ActorSystem System { get; }

        private readonly IActorRef _clientManager;

        public AkkaExchange()
        {
            System = ActorSystem.Create("akka-exchange-system");

            _clientManager = System.ActorOf(System.DI().Props<ClientManagerActor>(), "client-manager");
        }

        public void Dispose()
        {
            System.Dispose();
        }

        public async Task<AkkaExchangeClient> NewConnection()
        {
            var command = new StartConnectionCommand();
            var responseObject = await _clientManager.Ask(command);
            return new AkkaExchangeClient(responseObject as IActorRef);
        }

        public async Task EndConnection(Guid connectionId)
        {
            var command = new EndConnectionCommand(connectionId);
            var responseObject = await _clientManager.Ask(command);
        }
    }
}