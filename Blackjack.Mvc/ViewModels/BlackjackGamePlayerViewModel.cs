using Blackjack.Domain;
using System;

namespace Blackjack.Mvc.ViewModels
{
    public class BlackjackGamePlayerViewModel
    {
        public string Id { get; private set; }
        public string Name { get; private set; }
        public string Balance { get; private set; }
        public string Wager { get; private set; }
        public bool IsLive { get; private set; }
        public bool HasAction { get; private set; }
        public BlackjackHandViewModel Hand { get; private set; }
        public int Position { get; private set; }
        public string RecentWagerOutcome { get; private set; }
        public int SecondsAwaitingAction { get; private set; }

        public BlackjackGamePlayerViewModel(BlackjackGamePlayer player, int secondsAwaitingAction, BlackjackHandSettlement settlement = null)
        {
            if (player == null)
                throw new ArgumentNullException("player");

            Id = player.Id;
            Name = player.Alias;
            Balance = player.Account.Balance.ToString();            
            IsLive = player.IsLive;
            HasAction = player.HasAction;
            Position = player.Position;
            SecondsAwaitingAction = secondsAwaitingAction;

            BlackjackHand hand;
            if (settlement != null)
            {
                hand = settlement.PlayerHand;
                Wager = settlement.WagerAmount.ToString();
                RecentWagerOutcome = settlement.WagerOutcome.ToString();
            }
            else
            {
                hand = player.Hand;
                Wager = player.Wager.ToString();
                RecentWagerOutcome = "";
            }
            Hand = new BlackjackHandViewModel(hand);
        }
    }
}