using System;

namespace AkkaExchange.Client.Commands
{
    public class StartConnectionCommand : ICommand
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