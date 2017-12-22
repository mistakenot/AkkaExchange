using System;
using Akka.Persistence;
using AkkaExchange.Events;
using AkkaExchange.Handlers;
using AkkaExchange.State;

namespace AkkaExchange.Actors
{
    public class BaseActor<TState, THandler> : UntypedPersistentActor
        where THandler : class, ICommandHandler<TState>
        where TState : class, IState<TState>
    {
        public override string PersistenceId { get; }

        public BaseActor(THandler handler, TState state, string persistenceId)
        {
            PersistenceId = persistenceId;
            _handler = handler ?? throw new ArgumentNullException(nameof(handler));
            _state = state ?? throw new ArgumentNullException(nameof(state));
        }

        private readonly THandler _handler;
        private TState _state;

        protected override void OnCommand(object message)
        {
            var handlerResult = _handler.Handle(_state, message);

            if (handlerResult.Success)
            {
                Persist(handlerResult.Event, OnPersist);
            }
            else
            {
                // Send errors to client.
            }
        }

        protected override void OnRecover(object persistedEvent)
        {
            if (persistedEvent is IEvent evnt)
            {
                _state = _state.Update(evnt);
            }
        }

        private void OnPersist<T>(T persistedEvent)
        {
            if (persistedEvent is IEvent evnt)
            {
                _state = _state.Update(evnt);
            }

            Sender.Tell(persistedEvent, Self);
        }
    }
}
