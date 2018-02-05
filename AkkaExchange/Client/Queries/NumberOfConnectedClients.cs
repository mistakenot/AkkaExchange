namespace AkkaExchange.Client.Queries
{
    public class NumberOfConnectedClients : Message
    {
        public int ConnectedClients { get; }

        public NumberOfConnectedClients(int numberOfClients)
        {
            ConnectedClients = numberOfClients;
        }
    }
}