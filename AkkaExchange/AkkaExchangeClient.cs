using System;
using System.Threading.Tasks;
using Akka.Actor;
using AkkaExchange.Client.Commands;
using AkkaExchange.Orders;
using AkkaExchange.Orders.Commands;

namespace AkkaExchange
{
    public class AkkaExchangeClient : IDisposable
    {
        private readonly Guid _clientId;
        private readonly Inbox _inbox;
        private readonly IActorRef _actor;

        internal AkkaExchangeClient(
            Guid clientId,
            IActorRef actor,
            Inbox inbox)
        {
            _clientId = clientId;
            _inbox = inbox ?? throw new ArgumentNullException(nameof(inbox));
            _actor = actor ?? throw new ArgumentNullException(nameof(actor));
        }

        public void NewOrder(decimal price, decimal amount, OrderStateSide side)
        {
            var command = new NewOrderCommand(
                new Order(_clientId, amount, price, side));

            _actor.Tell(command, _inbox.Receiver);
        }

        public void AmendOrder(Guid orderId, decimal price, decimal amount, OrderStateSide side)
        {
            var command = new AmendOrderCommand(
                orderId,
                new Order(_clientId, orderId, amount, price, side));

            _actor.Tell(command, _inbox.Receiver);
        }

        public void CancelOrder(Guid orderId)
        {
            var command = new RemoveOrderCommand(orderId);
            _actor.Tell(command, _inbox.Receiver);
        }

        public void Dispose()
        {
            var command = new EndConnectionCommand(_clientId);

            _actor.Tell(command);
            _inbox.Dispose();
        }

        public async Task<object> TakeNextMessage()
        {
            return await _inbox.ReceiveAsync();
        }
    }
}