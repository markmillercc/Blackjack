using Blackjack.Mvc.Models;
using System.Linq;

namespace Blackjack.Mvc.ViewModels
{
    public class BlackjackGameAsListItemViewModel
    {
        public string GameId { get; private set; }
        public string Title { get; private set; }
        public string OpenSeats { get; private set; }
        public string Description { get; private set; }

        public BlackjackGameAsListItemViewModel(MyBlackjackGame game)
        {
            GameId = game.Id;
            Title = game.Title;
            Description = string.Format("Blackjack ${0} - ${1}", game.MinWager, game.MaxWager);
            OpenSeats = (game.MaxPlayers - game.Players.Count()).ToString();
        }
    }
}