using Microsoft.AspNetCore.SignalR;

namespace TodorovNet.Hubs
{
    public class TimingHub : Hub
    {
        public Task SubscribeToEvent(string eventId)
            => Groups.AddToGroupAsync(Context.ConnectionId, $"event_{eventId}");

        public Task UnsubscribeFromEvent(string eventId)
            => Groups.RemoveFromGroupAsync(Context.ConnectionId, $"event_{eventId}");
    }
}
