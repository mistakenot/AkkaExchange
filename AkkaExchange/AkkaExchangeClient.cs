﻿using System;
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
        public Guid ClientId { get; }
        
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
            ClientId = clientId;
            _inbox = inbox ?? throw new ArgumentNullException(nameof(inbox));
            _clientActor = clientActor ?? throw new ArgumentNullException(nameof(clientActor));

            State = state ?? throw new ArgumentNullException(nameof(state));
            Errors = errors ?? throw new ArgumentNullException(nameof(errors));
            Events = events ?? throw new ArgumentNullException(nameof(events));
        }

        public void NewOrder(decimal price, decimal amount, OrderSide side)
        {
            var command = new ExecuteOrderCommand(
                ClientId, 
                new NewOrderCommand(
                    new Order(ClientId, amount, price, side)));

            _clientActor.Tell(command, _inbox.Receiver);
        }

        public void AmendOrder(Guid orderId, decimal price, decimal amount, OrderSide side)
        {
            var command = new ExecuteOrderCommand(
                ClientId,
                new AmendOrderCommand(
                    orderId,
                    new Order(ClientId, amount, price, side)));

            _clientActor.Tell(command, _inbox.Receiver);
        }

        public void CancelOrder(Guid orderId)
        {
            var command = new ExecuteOrderCommand(
                ClientId,
                new RemoveOrderCommand(orderId));

            _clientActor.Tell(command, _inbox.Receiver);
        }

        public void Dispose()
        {
            var command = new EndConnectionCommand(ClientId);

            _clientActor.Tell(command);
            _inbox.Dispose();
        }
    }
}