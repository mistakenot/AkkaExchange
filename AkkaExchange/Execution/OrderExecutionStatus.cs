namespace AkkaExchange.Execution
{
    public enum OrderExecutionStatus
    {
        Pending = 0,
        InProgress = 1,
        Error = 2,
        Complete = 3,
    }
}