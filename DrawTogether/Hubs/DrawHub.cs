using Microsoft.AspNetCore.SignalR;

namespace DrawTogether.Hubs
{
    public class DrawHub : Hub
    {
        public async Task SendDraw(float startX, float startY, float endX, float endY)
        {
            await Clients.Others.SendAsync("ReceiveDraw", startX, startY, endX, endY);
        }
    }
}
