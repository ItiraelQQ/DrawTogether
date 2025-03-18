namespace DrawTogether.Services
{
    public class MatchmakingService
    {
        private static readonly object _lock = new();
        private static string _waitingUser = null;
        private static Dictionary<string, string> _rooms = new(); // UserId -> RoomId
    
        public (bool isPaired, string roomId) FindMatch(string userId)
        {
            lock (_lock)
            {
                if (_waitingUser == null)
                {
                    _waitingUser = userId;
                    return (false, null); // waiting for player 2
                }
                else
                {
                    string roomId = Guid.NewGuid().ToString();
                    _rooms[_waitingUser] = roomId;
                    _rooms[userId] = roomId;
                    _waitingUser = null;
                    return (true, roomId);
                }
            }
        }
        public string GetRoomForUser(string userId)
        {
            return _rooms.ContainsKey(userId) ? _rooms[userId] : null;
        }
    }

   
}
