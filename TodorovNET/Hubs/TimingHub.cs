using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace TodorovNet.Hubs
{
    public class TimingHub : Hub
    {
        // 🔹 Клиентът се абонира за дадено събитие (примерно eventId=1)
        public Task SubscribeToEvent(string eventId)
        {
            return Groups.AddToGroupAsync(Context.ConnectionId, $"event_{eventId}");
        }

        // 🔹 Клиентът се отписва (ако смени събитие или затвори страницата)
        public Task UnsubscribeFromEvent(string eventId)
        {
            return Groups.RemoveFromGroupAsync(Context.ConnectionId, $"event_{eventId}");
        }
    }
}
