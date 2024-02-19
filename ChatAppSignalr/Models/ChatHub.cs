using Microsoft.AspNetCore.SignalR;

namespace ChatAppSignalr.Models
{
    public class ChatHub:Hub
    {
        private static readonly Dictionary<string, List<Message>> GroupMessages = new Dictionary<string, List<Message>>();

        public async Task CreateGroup(string groupName)
        {
            if (!GroupMessages.ContainsKey(groupName))
            {
                GroupMessages[groupName] = new List<Message>();
                await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
                await Clients.All.SendAsync("GroupCreated", groupName);
            }
            else
            {
                await Clients.Caller.SendAsync("GroupExists", groupName);
            }
        }

        public async Task SendMessageToGroup(string groupName, string user, string message)
        {
            if (GroupMessages.ContainsKey(groupName))
            {
                var newMessage = new Message { User = user, Text = message };
                GroupMessages[groupName].Add(newMessage);
                await Clients.Group(groupName).SendAsync("ReceiveMessage", newMessage);
            }
        }

        public async Task DeleteGroup(string groupName)
        {
            if (GroupMessages.ContainsKey(groupName))
            {
                GroupMessages.Remove(groupName);
                await Clients.All.SendAsync("GroupDeleted", groupName);
            }
        }

        public async Task GetGroupMessages(string groupName)
        {
            if (GroupMessages.ContainsKey(groupName))
            {
                var messages = GroupMessages[groupName];
                await Clients.Caller.SendAsync("LoadGroupMessages", messages);
            }
        }

        private class Message
        {
            public string User { get; set; }
            public string Text { get; set; }
        }
    }

}
