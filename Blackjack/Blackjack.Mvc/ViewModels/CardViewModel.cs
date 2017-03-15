using Blackjack.Domain;

namespace Blackjack.Mvc.ViewModels
{
    public class CardViewModel
    {
        public string Rank { get; private set; }
        public string Suit { get; private set; }
        public string SuitCode { get; private set; }

        public CardViewModel(Card card)
        {
            if (card.Rank >= CardRank.Two && card.Rank <= CardRank.Ten)
            {
                Rank = ((int)card.Rank).ToString();
            }
            else
            {
                Rank = card.Rank.ToString().Substring(0, 1);
            }

            switch (card.Suit)
            {
                case CardSuit.Club:
                    Suit = "clubs";
                    SuitCode = "&clubs;";
                    break;
                case CardSuit.Diamond:
                    Suit = "diams";
                    SuitCode = "&diams;";
                    break;
                case CardSuit.Heart:
                    Suit = "hearts";
                    SuitCode = "&hearts;";
                    break;
                case CardSuit.Spade:
                    Suit = "spades";
                    SuitCode = "&spades;";
                    break;
            }
        }
    }
}