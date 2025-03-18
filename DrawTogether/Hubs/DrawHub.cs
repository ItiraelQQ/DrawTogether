using DrawTogether.Services;
using Microsoft.AspNetCore.SignalR;

namespace DrawTogether.Hubs
{
    public class DrawHub : Hub
    {
        private readonly MatchmakingService _matchmaking;

        private static Dictionary<string, string> UserConnections = new();
        private static List<string> WaitingUsers = new();

        public DrawHub(MatchmakingService matchmaking)
        {
            _matchmaking = matchmaking;
        }

        public async Task StartMatchmaking(string userId)
        {
            if (!UserConnections.ContainsKey(userId))
                UserConnections.Add(userId, Context.ConnectionId);
            else
                UserConnections[userId] = Context.ConnectionId;

            if (!WaitingUsers.Contains(userId))
            {
                WaitingUsers.Add(userId);
                await Clients.Caller.SendAsync("WaitingForPartner");
            }

            if (WaitingUsers.Count >= 2)
            {
                var user1 = WaitingUsers[0];
                var user2 = WaitingUsers[1];
                WaitingUsers.RemoveRange(0, 2);

                var roomId = Guid.NewGuid().ToString();

                var conn1 = UserConnections[user1];
                var conn2 = UserConnections[user2];

                await Clients.Client(conn1).SendAsync("MatchFound", roomId);
                await Clients.Client(conn2).SendAsync("MatchFound", roomId);
            }
        }


        public async Task JoinRoom(string roomId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
        }

        public async Task SendDraw(string roomId, float startX, float startY, float endX, float endY)
        {
            await Clients.Group(roomId).SendAsync("ReceiveDraw", startX, startY, endX, endY);
        }

        public override Task OnConnectedAsync()
        {
            Console.WriteLine($"Client connected: {Context.ConnectionId}");
            return base.OnConnectedAsync();
        }
    }
}
