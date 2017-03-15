using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blackjack.Domain
{
    public class BlackjackGameRoundPlayer
    {
        private List<Card> _cards;

        public BlackjackGamePlayer Player { get; private set; }
        public BlackjackHand Hand { get; private set; }
        public double Wager { get; set; }
        public bool HasAction { get; set; }        

        public BlackjackGameRoundPlayer(BlackjackGamePlayer player, double wager)
        {
            if (player == null)
                throw new ArgumentNullException("player", "Player is null");

            if (wager <= 0)
                throw new InvalidOperationException("Wager cannot be negative");

            _cards = new List<Card>();
            Player = player;
            Wager = wager;
        }

        public void AddCardToHand(Card card)
        {
            if (card != null)
            {
                _cards.Add(card);
                if (_cards.Count() >= 2)
                    Hand = new BlackjackHand(_cards);
            }
        }
    }
}
