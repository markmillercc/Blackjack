using System;
using System.Collections.Generic;
using System.Linq;

namespace Blackjack.Domain
{
    public class BlackjackGameRound
    {
        private List<BlackjackGameRoundPlayer> _roundPlayers;
        public IEnumerable<BlackjackGameRoundPlayer> RoundPlayers { get { return _roundPlayers.ToList(); } }

        private List<Card> _dealerCards;
        public BlackjackHand DealerHand { get; private set; }

        private List<BlackjackHandSettlement> _settlements;
        public IEnumerable<BlackjackHandSettlement> Settlements { get { return _settlements; } }

        public bool IsInitialized { get; private set; }

        public BlackjackGameRound(IEnumerable<BlackjackGameRoundPlayer> roundplayers)
        {
            if (roundplayers == null || !roundplayers.Any())
                throw new InvalidOperationException("At least one player required to create a new round");

            _dealerCards = new List<Card>();
            _settlements = new List<BlackjackHandSettlement>();

            _roundPlayers = roundplayers.OrderBy(a => a.Player.Position).ToList();
            _roundPlayers.ForEach(player =>
            {
                player.HasAction = false;
            });

            IsInitialized = false;            
        }

        public void InitializeAction()
        {
            if (IsInitialized)
                return;

            if (_roundPlayers.Any(a => a.HasAction))
                throw new InvalidOperationException();

            if (_roundPlayers.Any())
                _roundPlayers.First().HasAction = true;

            IsInitialized = true;
        }

        public bool MoveToNextAction()
        {
            var currentPlayerWithAction = _roundPlayers.SingleOrDefault(a => a.HasAction);
            if (currentPlayerWithAction == null)
                return false;

            currentPlayerWithAction.HasAction = false;

            var nextPlayerWithAction = _roundPlayers
                .Where(a => a.Player.Position > currentPlayerWithAction.Player.Position)
                .OrderBy(a => a.Player.Position)
                .FirstOrDefault();

            if (nextPlayerWithAction == null)
                return false;

            nextPlayerWithAction.HasAction = true;
            return true;
        }

        public void AddCardToDealerHand(Card card)
        {
            if (card != null)
            {
                _dealerCards.Add(card);
                if (_dealerCards.Count() >= 2)
                    DealerHand = new BlackjackHand(_dealerCards);
            }
        }

        public BlackjackGameRoundPlayer GetRoundPlayer(BlackjackGamePlayer player)
        {
            return _roundPlayers.SingleOrDefault(a => a.Player.Id == player.Id);
        }

        public void SettleRoundPlayer(BlackjackGameRoundPlayer roundplayer, BlackjackHandSettlement settlement)
        {
            _settlements.Add(settlement);
            _roundPlayers.Remove(roundplayer);
        }
    }
}
