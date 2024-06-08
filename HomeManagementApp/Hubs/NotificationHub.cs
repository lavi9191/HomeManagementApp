using Microsoft.AspNetCore.SignalR;

public class NotificationHub : Hub
{
    public async Task SendMessage(string user, string message)
    {
        await Clients.User(user).SendAsync("ReceiveMessage", message);
    }
}
