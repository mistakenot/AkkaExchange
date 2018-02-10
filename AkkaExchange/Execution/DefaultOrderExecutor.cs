using System;
using System.Threading.Tasks;
using AkkaExchange.Orders;

namespace AkkaExchange.Execution
{
    public class DefaultOrderExecutor : IOrderExecutor
    {
        public async Task Execute(OrderMatch matchedOrders)
        {
            // A perfect world where everything works...
            await Task.Delay(100);
        }
    }
}