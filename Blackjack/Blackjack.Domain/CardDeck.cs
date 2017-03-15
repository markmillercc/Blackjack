using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace Blackjack.Domain
{
    public class CardDeck
    {
        protected List<Card> _cards;
        public IEnumerable<Card> Cards { get { return _cards; } }

        protected CardDeck()
        {
            _cards = new List<Card>();
        }

        public CardDeck(bool shuffled) : this()
        {
            var ranks = Enum.GetValues(typeof(CardRank)).Cast<CardRank>().ToList();
            var suits = Enum.GetValues(typeof(CardSuit)).Cast<CardSuit>().ToList();

            suits.ForEach(suit =>
                ranks.ForEach(rank =>
                    _cards.Add(new Card(rank, suit))));

            if (shuffled)
                Shuffle();
        }

        public virtual void Shuffle()
        {
            _cards = _cards.OrderBy(o => Guid.NewGuid()).ToList();
        }

        public virtual Card DealCard()
        {
            if (!_cards.Any())
                throw new InvalidOperationException("Deck is out of cards");

            var card = _cards.ElementAt(0);
            _cards.RemoveAt(0);
            return card;
        }
    }
}
