using System;
using Akka.TestKit;
using Akka.TestKit.Xunit2;
using Xunit;

namespace AkkaExchange.Tests
{
    public class AkkaExchangeActorTestFixture : TestKit
    {
        protected TestProbe Probe { get; set; }

        public AkkaExchangeActorTestFixture()
        {
            Probe = CreateTestProbe();
        }

        public T WaitForOne<T>()
        {
            Within(TimeSpan.FromSeconds(1), () => Probe.HasMessages);
            return Probe.ExpectMsg<T>();
        }
    }
}