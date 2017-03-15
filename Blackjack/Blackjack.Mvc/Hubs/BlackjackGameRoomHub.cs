using Microsoft.AspNet.SignalR;

namespace Blackjack.Mvc.Hubs
{
    public class BlackjackGameRoomHub : Hub
    {
        public void Subscribe(string groupName)
        {
            Groups.Add(Context.ConnectionId, groupName);
        }

        public void Unsubscribe(string groupName)
        {
            Groups.Remove(Context.ConnectionId, groupName);
        }

        public void SendMessage(string gameid, string senderName, string message, int seatnumber)
        {
            Clients.Group(gameid).sendMessage(senderName, message, seatnumber);
        }

        public void ShowPlayerIsTyping(string gameid, int seatnumber)
        {
            Clients.Group(gameid).showPlayerIsTyping(seatnumber);
        }
    }
}