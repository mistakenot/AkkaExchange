using System;

namespace AkkaExchange.Client.Commands
{
    public class EndConnectionCommand : Message, ICommand
    {
        public Guid ClientId { get; }

        public EndConnectionCommand(Guid clientId)
        {
            ClientId = clientId;
        }
    }
}