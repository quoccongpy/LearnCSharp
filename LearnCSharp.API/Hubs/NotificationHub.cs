using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace LearnCSharp.API.Hubs
{
    [Authorize]
    public class NotificationHub : Hub
    {
        
    }
}