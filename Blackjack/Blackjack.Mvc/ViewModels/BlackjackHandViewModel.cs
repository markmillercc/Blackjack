using Blackjack.Domain;
using System.Collections.Generic;
using System.Linq;

namespace Blackjack.Mvc.ViewModels
{
    public class BlackjackHandViewModel
    {
        public IEnumerable<CardViewModel> Cards { get; private set; }        
        public bool IsBusted { get; private set; }
        public bool IsBlackjack { get; private set; }
        public bool IsSoft { get; private set; }
        public string Score { get; private set; }

        public BlackjackHandViewModel(BlackjackHand hand)
        {
            IsBlackjack = hand?.IsBlackjack ?? false;
            IsBusted = hand?.IsBusted ?? false;
            IsSoft = hand?.IsSoft ?? false;

            Cards = hand?.Cards?.Select(card => new CardViewModel(card))?.ToList() 
                ?? Enumerable.Empty<CardViewModel>();

            if (IsBusted)
            {
                Score = "Busted";
            }
            else if (IsBlackjack)
            {
                Score = "Blackjack";
            }
            else if (IsSoft)
            {
                Score = hand.ScoreHighLow.Item1.ToString();
                if (hand.ScoreHighLow.Item2 <= 21)
                    Score += " or " + hand.ScoreHighLow.Item2.ToString();
            }
            else
            {
                Score = hand == null ? "" : hand.Score.ToString();
            }            
        }
    }
}