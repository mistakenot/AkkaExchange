using System;
using System.Threading.Tasks;
using AkkaExchange.Orders;

namespace AkkaExchange.Execution
{
    public interface IOrderExecutor
    {
        Task Execute(OrderMatch matchedOrders);
    }
}