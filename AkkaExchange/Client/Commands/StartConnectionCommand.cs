using System;

namespace AkkaExchange.Client.Commands
{
    public class StartConnectionCommand : Message, ICommand
    {
        public Guid ClientId { get; }

        public StartConnectionCommand()
            : this(Guid.NewGuid())
        {
            
        }

        public StartConnectionCommand(Guid clientId)
        {
            ClientId = clientId;
        }
    }
}