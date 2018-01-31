namespace AkkaExchange
{
    public class Message : IMessage
    {
        public string Type => this.GetType().Name;
    }
}