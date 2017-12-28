using System;
using AkkaExchange.Orders;
using AkkaExchange.Utils;
using Xunit;

namespace AkkaExchange.Tests.Orders
{
    public abstract class OrderFixture
    {
        protected static PlacedOrder Ask(decimal amount, decimal price)
            => new PlacedOrder(
                new Order(Guid.NewGuid(), amount, price, OrderSide.Ask));

        protected static PlacedOrder Bid(decimal amount, decimal price)
            => new PlacedOrder(
                new Order(Guid.NewGuid(), amount, price, OrderSide.Bid));

        protected static TEvent AssertSuccess<TEvent>(HandlerResult result)
        {
            Assert.True(result.Success);
            Assert.True(result.WasHandled);
            Assert.Empty(result.Errors);
            return Assert.IsType<TEvent>(result.Event);
        }

    }
}