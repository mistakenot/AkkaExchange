using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace AkkaExchange.Web
{
    public class AkkaExchangeHub : Hub
    {
        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }
    }
}