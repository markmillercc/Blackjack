using Blackjack.Domain;
using System;
using System.Linq;

namespace Blackjack.Mvc.ViewModels
{
    public class BlackjackGameDealerViewModel
    {
        public string Name { get; private set; }
        public BlackjackHandViewModel Hand { get; private set; }
        public bool CanShowHand { get; private set; }
        public int PercentOfCardsRemainingInShoe { get; private set; }

        public BlackjackGameDealerViewModel(BlackjackGame game)
        {
            if (game == null)
                throw new ArgumentNullException("game");

            Name = "Dealer";
            Hand = new BlackjackHandViewModel(game.DealerHand);
            CanShowHand = game.Players.All(a => !a.HasAction);
            PercentOfCardsRemainingInShoe = game.PercentRemainingInDealerShoe;
        }
    }
}