using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blackjack.Domain
{
    public class BlackjackHand
    {
        public IEnumerable<Card> Cards { get; private set; }               
        public Tuple<int, int> ScoreHighLow { get; private set; }
        public int Score { get; private set; }
        public bool IsBlackjack { get; private set; }
        public bool IsBusted { get; private set; }
        public bool IsSoft { get; private set; }

        public BlackjackHand(IEnumerable<Card> cards)
        {
            Cards = cards.ToList();

            ScoreHighLow = CalculateScoreHighLow();

            var low = ScoreHighLow.Item1;
            var high = ScoreHighLow.Item2;

            Score = high > 21 ? low : high;

            IsBlackjack = Cards.Count() == 2 && Score == 21;
            IsBusted = Score > 21;
            IsSoft = Cards.Any(a => a.Rank == CardRank.Ace);
        }

        private Tuple<int, int> CalculateScoreHighLow()
        {
            var aceCount = 0;
            var score = 0;

            foreach (var card in Cards)
            {
                if (card.Rank == CardRank.Ace)
                {
                    aceCount++;
                }
                else if (card.Rank >= CardRank.Ten)
                {
                    score += 10;
                }
                else
                {
                    score += (int)card.Rank;
                }
            }

            if (aceCount == 0)
                return new Tuple<int, int>(score, score);

            var lowScore = score + aceCount;
            var highScore = lowScore + 10;

            return new Tuple<int, int>(lowScore, highScore);
        }
    }

}
