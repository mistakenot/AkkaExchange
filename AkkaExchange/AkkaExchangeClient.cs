using System;
using Akka.Actor;
using AkkaExchange.Client;
using AkkaExchange.Client.Commands;
using AkkaExchange.Orders;
using AkkaExchange.Orders.Commands;
using AkkaExchange.Utils;

namespace AkkaExchange
{
    public class AkkaExchangeClient : IDisposable
    {
        public IObservable<ClientState> State { get; }
        public IObservable<HandlerResult> Errors { get; }
        public IObservable<object> Events { get; }

        private readonly Guid _clientId;
        private readonly Inbox _inbox;
        private readonly IActorRef _clientActor;

        internal AkkaExchangeClient(
            Guid clientId,
            Inbox inbox,
            IActorRef clientActor, 
            IObservable<object> events,
            IObservable<ClientState> state,
            IObservable<HandlerResult> errors)
        {
            _clientId = clientId;
            _inbox = inbox ?? throw new ArgumentNullException(nameof(inbox));
            _clientActor = clientActor ?? throw new ArgumentNullException(nameof(clientActor));

            State = state ?? throw new ArgumentNullException(nameof(state));
            Errors = errors ?? throw new ArgumentNullException(nameof(errors));
            Events = events ?? throw new ArgumentNullException(nameof(events));
        }

        public void NewOrder(decimal price, decimal amount, OrderSide side)
        {
            var command = new ExecuteOrderCommand(
                _clientId, 
                new NewOrderCommand(
                    new Order(_clientId, amount, price, side)));

            _clientActor.Tell(command, _inbox.Receiver);
        }

        public void AmendOrder(Guid orderId, decimal price, decimal amount, OrderSide side)
        {
            var command = new ExecuteOrderCommand(
                _clientId,
                new AmendOrderCommand(
                    orderId,
                    new Order(_clientId, amount, price, side)));

            _clientActor.Tell(command, _inbox.Receiver);
        }

        public void CancelOrder(Guid orderId)
        {
            var command = new ExecuteOrderCommand(
                _clientId,
                new RemoveOrderCommand(orderId));

            _clientActor.Tell(command, _inbox.Receiver);
        }

        public void Dispose()
        {
            var command = new ExecuteOrderCommand(
                _clientId,
                new EndConnectionCommand(_clientId));

            _clientActor.Tell(command);
            _inbox.Dispose();
        }
    }
}