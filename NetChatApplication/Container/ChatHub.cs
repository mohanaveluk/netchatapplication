using Microsoft.AspNetCore.SignalR;
using NetChatApplication.Models;

namespace NetChatApplication.Container
{
    public class ChatHub: Hub
    {
        private readonly IDictionary<string, UserRoomConnection>? _connection;

        public ChatHub(IDictionary<string, UserRoomConnection>? connection)
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

        public async Task SendMessage(string sender, string receiver, string message)
        {
            if (_connection.TryGetValue(Context.ConnectionId, out UserRoomConnection? userRoomConnection))
            {
                await Clients.Group(userRoomConnection.Room!)
                    .SendAsync("ReceiveMessage", userRoomConnection.User, message, DateTime.Now);

                await Clients.User(receiver).SendAsync("ReceiveMessage", userRoomConnection.Email, message, DateTime.Now);
            }
        }

        public override Task OnDisconnectedAsync(Exception? exp)
        {
            if (!_connection.TryGetValue(Context.ConnectionId, out UserRoomConnection? roomConnection))
            {
                return base.OnDisconnectedAsync(exp);
            }
            Groups.RemoveFromGroupAsync(Context.ConnectionId, roomConnection.Room!);
            _connection.Remove(Context.ConnectionId);
            Clients.Group(roomConnection.Room!)
                .SendAsync("ReceiveMessage", "Lets Program bot", $"{roomConnection.User} has Left the Group", DateTime.Now);
            SendConnectedUser(roomConnection.Room!);
            return base.OnDisconnectedAsync(exp);
        }

        public Task SendConnectedUser(string room)
        {
            var users = _connection?.Values
                .Where(u => u.Room == room || u.Email == room)
                .Select(s => s.User);
            return Clients.Group(room).SendAsync("ConnectedUser", users);
        }
    }
}
