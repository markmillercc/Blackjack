namespace Blackjack.Domain
{
    public class Card
    {
        public CardRank Rank { get; private set; }
        public CardSuit Suit { get; private set; }

        public Card(CardRank rank, CardSuit suit)
        {
            Rank = rank;
            Suit = suit;
        }
    }

    public enum CardRank
    {
        Ace = 1,
        Two = 2,
        Three = 3,
        Four = 4,
        Five = 5,
        Six = 6,
        Seven = 7,
        Eight = 8,
        Nine = 9,
        Ten = 10,
        Jack = 11,
        Queen = 12,
        King = 13
    }

    public enum CardSuit
    {
        Club,
        Diamond,
        Heart,
        Spade
    }
}
