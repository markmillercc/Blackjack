using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Blackjack.Domain
{
    public class BlackjackGameDealer
    {
        private CardShoe _shoe;
        public BlackjackGameRound RoundInProgress { get; set; }
        
        public int PercentRemainingInShoe { get { return _shoe.PercentRemaining; } }

        public BlackjackGameDealer()
        {
            _shoe = new CardShoe(1);
        }

        public void Deal(BlackjackGameRound round)
        {
            if (RoundInProgress != null)
                throw new InvalidOperationException("Round already in play");

            if (round == null)
                throw new ArgumentNullException("round", "Round is null");

            if (round.IsInitialized)
                throw new InvalidOperationException("Round already initialized");

            RoundInProgress = round;

            for (int i = 0; i < 2; i++)
            {
                RoundInProgress.RoundPlayers.ToList()
                    .ForEach(e => e.AddCardToHand(_shoe.DealCard()));

                RoundInProgress.AddCardToDealerHand(_shoe.DealCard());
            }

            if (!RoundInProgress.DealerHand.IsBlackjack)
                RoundInProgress.InitializeAction();
        }

        public void HandleRequestToHit(BlackjackGamePlayer player)
        {
            if (RoundInProgress == null)
                throw new InvalidOperationException("No round in play");

            var roundplayer = RoundInProgress.GetRoundPlayer(player);
            if (roundplayer == null)
                throw new ArgumentException("'player' is null or invalid");

            if (!roundplayer.HasAction || roundplayer.Hand == null)
                throw new InvalidOperationException("Out of turn");

            if (roundplayer.Hand.IsBusted)
                throw new InvalidOperationException("Cannot hit busted hand");

            roundplayer.AddCardToHand(_shoe.DealCard());

            if (roundplayer.Hand.IsBusted)            
                moveToNextAction();            
        }

        public void HandleRequestToStand(BlackjackGamePlayer player)
        {
            if (RoundInProgress == null)
                throw new InvalidOperationException("No round in play");

            var roundplayer = RoundInProgress.GetRoundPlayer(player);
            if (roundplayer == null)
                throw new ArgumentException("'player' is null or invalid");

            if (!roundplayer.HasAction || roundplayer.Hand == null)
                throw new InvalidOperationException("Out of turn");

            moveToNextAction();
        }

        public void HandleRequestToDoubleDown(BlackjackGamePlayer player, double amount)
        {
            if (RoundInProgress == null)
                throw new InvalidOperationException("No round in play");

            var roundplayer = RoundInProgress.GetRoundPlayer(player);
            if (roundplayer == null)
                throw new ArgumentException("'player' is null or invalid");

            if (!roundplayer.HasAction || roundplayer.Hand == null)
                throw new InvalidOperationException("Out of turn");

            if (roundplayer.Hand.Cards.Count() != 2)
                throw new InvalidOperationException("Must have 2 cards to double");

            if (amount > roundplayer.Wager)
                throw new InvalidOperationException("Amounts exceeds current wager");

            roundplayer.AddCardToHand(_shoe.DealCard());
            roundplayer.Wager += amount;
            roundplayer.Player.Account.Debit(amount);

            moveToNextAction();
        }

        public void RefreshShoe()
        {
            if (RoundInProgress != null)
                throw new InvalidOperationException("Round is in play");

            _shoe = new CardShoe(8);
        }

        public void CloseRound()
        {
            if (RoundInProgress != null)
            {
                if (RoundInProgress.RoundPlayers.Any(a => a.HasAction))
                    throw new InvalidOperationException("Round still in progress");

                RoundInProgress.RoundPlayers.ToList()
                    .ForEach(player => settleHand(player));

                RoundInProgress = null;
            }                                   
        }

        private void moveToNextAction()
        {
            if (!RoundInProgress.MoveToNextAction())
                playDealerHand();
        }

        private void playDealerHand()
        {
            if (RoundInProgress == null)
                return;

            if (RoundInProgress.RoundPlayers.All(a => a.Hand.IsBusted))
                return;

            while (RoundInProgress.DealerHand != null)
            {
                if (RoundInProgress.DealerHand.Score > 17)
                    break;

                if (RoundInProgress.DealerHand.Score == 17 && !RoundInProgress.DealerHand.IsSoft)
                    break;

                RoundInProgress.AddCardToDealerHand(_shoe.DealCard());
            }
        }

        private BlackjackHandSettlement settleHand(BlackjackGameRoundPlayer roundplayer)
        {
            if (RoundInProgress.RoundPlayers.Any(a => a.HasAction))
            {
                if (!roundplayer.Hand.IsBusted)                    
                    throw new InvalidOperationException("Non-busted hands cannot settle until round completes");
            }

            var settlement = new BlackjackHandSettlement(roundplayer, RoundInProgress.DealerHand);

            if (settlement.WagerOutcome == WagerOutcome.Draw)
            {
                roundplayer.Player.Account.Credit(settlement.WagerAmount);
            }
            else if (settlement.WagerOutcome == WagerOutcome.Win)
            {
                roundplayer.Player.Account.Credit(settlement.WagerAmount * 2);
            }

            RoundInProgress.SettleRoundPlayer(roundplayer, settlement);

            return settlement;
        }

        public BlackjackHandSettlement SettleHand(BlackjackGamePlayer player)
        {
            var roundplayer = RoundInProgress.GetRoundPlayer(player);
            if (roundplayer == null)
                throw new ArgumentException("'player' is null or invalid");

            return settleHand(roundplayer);
        }
    }
}
