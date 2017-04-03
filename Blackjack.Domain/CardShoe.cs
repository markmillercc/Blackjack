using System;
using System.Linq;

namespace Blackjack.Domain
{
    public class CardShoe : CardDeck
    {
        public int NumberOfDecks { get; private set; }

        public int PercentRemaining
        {
            get
            {
                var percentRemaining = (decimal)Cards.Count() / (NumberOfDecks * 52);
                return (int)Math.Floor(percentRemaining * 100);
            }
        }

        public CardShoe(int numberOfDecks)
            : base()
        {
            if (numberOfDecks <= 0)
                throw new InvalidOperationException("Shoe must have at least one deck");

            NumberOfDecks = numberOfDecks;

            for (int i = 0; i < NumberOfDecks; i++)
            {
                var deck = new CardDeck(true);
                _cards.AddRange(deck.Cards);
            }

            Shuffle();
        }
    }
}
