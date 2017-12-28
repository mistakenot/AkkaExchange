using AkkaExchange.Execution;
using Xunit;

namespace AkkaExchange.Tests.Execution
{
    public class OrderExecutorManagerHandlerTests
    {
        private readonly OrderExecutorManagerHandler _subject;

        public OrderExecutorManagerHandlerTests()
        {
            _subject = new OrderExecutorManagerHandler();
        }

        [Fact]
        public void OrderExecutorManager_BeginOrderExecutionCommand_Ok()
        {
            
        }
    }
}