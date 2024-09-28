using Microsoft.AspNetCore.SignalR;

namespace ChatAppAPI.Hub
{
    public class ChatHub : Microsoft.AspNetCore.SignalR.Hub
    {
        private readonly IDictionary<string, UserRoomConnection> _connection;
        public ChatHub(IDictionary<string, UserRoomConnection> connection)
        {
            _connection = connection;
        }
        public async Task JoinRoom(UserRoomConnection userConnection)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, userConnection.Room!);
            _connection[Context.ConnectionId] = userConnection;
            await Clients.Group(userConnection.Room!)
                .SendAsync("ReceiveMessage", "Lets Program Bot", $"{userConnection.User} has Joined the Group", DateTime.Now);
            await SendConnectedUser(userConnection.Room!);
        }

        public async Task SendMessage(string message)
        {
            if (_connection.TryGetValue(Context.ConnectionId, out UserRoomConnection userRoomConnection))
            {
                await Clients.Group(userRoomConnection.Room!)
                    .SendAsync("RecieveMessage", userRoomConnection.User, message, DateTime.Now);
            }
        }

        public override Task OnDisconnectedAsync(Exception? exp)
        {
                if(!_connection.TryGetValue(Context.ConnectionId, out UserRoomConnection roomConnection))
            {
                return base.OnDisconnectedAsync(exp);
            }
                _connection.Remove(Context.ConnectionId);
            Clients.Group(roomConnection.Room!)
            .SendAsync("RecieveMessage", "Lets Program bot", $"{roomConnection.User} has left the Group", DateTime.Now);
            SendConnectedUser(roomConnection.Room!);
            return base.OnDisconnectedAsync(exp);
                
        }

        public Task SendConnectedUser(string room)
        {
            var users = _connection.Values
            .Where(u => u.Room == room)
            .Select(s => s.User);
            return Clients.Group(room).SendAsync("ConnectedUser", users);
        }
    }
}