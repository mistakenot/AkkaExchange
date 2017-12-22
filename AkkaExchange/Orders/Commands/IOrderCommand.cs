using AkkaExchange.Matching.Commands;
using AkkaExchange.State;

namespace AkkaExchange.Orders.Commands
{
    public interface IOrderCommand : ICommand
    {
        
    }

    public class OrderCommandHandler
    {
        public static ICommandHandler<ExchangeActorState> Instance = 
            new CommandHandlerCollection<ExchangeActorState>(new ICommandHandler<ExchangeActorState>[]
        {
            new AmendOrderCommandHandler(),
            new BeginMatchOrdersCommandHandler(), 
            new NewOrderCommandHandler(),
            new RemoveOrderCommandHandler(), 
        });
    }
}