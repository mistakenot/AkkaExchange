using System;
using System.Threading.Tasks;
using Akka.Actor;
using AkkaExchange.Client.Commands;
using AkkaExchange.Orders;
using AkkaExchange.Orders.Commands;
using AkkaExchange.Utils;

namespace AkkaExchange
{
    public class AkkaExchangeClient : IDisposable
    {
        public IObservable<object> Events { get; set; }

        private readonly Guid _clientId;
        private readonly Inbox _inbox;
        private readonly IActorRef _actor;
        private readonly Task _subscription;

        internal AkkaExchangeClient(
            Guid clientId, 
            IActorRef actor, 
            Task subscription, 
            Inbox inbox)
        {
            _clientId = clientId;
            _actor = actor ?? throw new ArgumentNullException(nameof(actor));
            _inbox = inbox ?? throw new ArgumentNullException(nameof(inbox));
            _subscription = subscription ?? throw new ArgumentNullException(nameof(subscription));

            Events = new InboxObservable(inbox);
        }

        public void NewOrder(decimal price, decimal amount, OrderSide side)
        {
            var command = new ExecuteOrderCommand(
                _clientId, 
                new NewOrderCommand(
                    new Order(_clientId, amount, price, side)));

            _actor.Tell(command, _inbox.Receiver);
        }

        public void AmendOrder(Guid orderId, decimal price, decimal amount, OrderSide side)
        {
            var command = new ExecuteOrderCommand(
                _clientId,
                new AmendOrderCommand(
                    orderId,
                    new Order(_clientId, orderId, amount, price, side)));

            _actor.Tell(command, _inbox.Receiver);
        }

        public void CancelOrder(Guid orderId)
        {
            var command = new ExecuteOrderCommand(
                _clientId,
                new RemoveOrderCommand(orderId));

            _actor.Tell(command, _inbox.Receiver);
        }

        public void Dispose()
        {
            var command = new ExecuteOrderCommand(
                _clientId,
                new EndConnectionCommand(_clientId));

            _actor.Tell(command);
            _inbox.Dispose();

            if (_subscription.IsCompleted)
            {
                _subscription.Dispose();
            }
        }
    }
}