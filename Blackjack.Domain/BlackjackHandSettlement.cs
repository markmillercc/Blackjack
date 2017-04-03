using System;

namespace Blackjack.Domain
{
    public class BlackjackHandSettlement
    {
        public int PlayerPosition { get; private set; }
        public BlackjackHand PlayerHand { get; private set; }
        public BlackjackHand DealerHand { get; private set; }
        public WagerOutcome WagerOutcome { get; private set; }
        public double WagerAmount { get; private set; }

        public BlackjackHandSettlement(BlackjackGameRoundPlayer player, BlackjackHand dealerHand)
        {
            if (player == null)
                throw new ArgumentNullException("player");

            if (player.Hand == null)
                throw new ArgumentNullException("player.hand");

            if (dealerHand == null)
                throw new ArgumentNullException("dealerHand");

            DealerHand = dealerHand;
            PlayerHand = player.Hand;            
            PlayerPosition = player.Player.Position;
            WagerAmount = player.Wager;
            WagerOutcome = GetOutcome();            
        }

        private WagerOutcome GetOutcome()
        {
            var score = PlayerHand.Score;
            var dealerScore = DealerHand.Score;

            if (score > 21)
                return WagerOutcome.Lose;

            if (dealerScore > 21)
                return WagerOutcome.Win;

            if (score == dealerScore)
                return WagerOutcome.Draw;

            return score > dealerScore ? WagerOutcome.Win : WagerOutcome.Lose;
        }
    }
}
