using Blackjack.Mvc.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Blackjack.Mvc.ViewModels
{
    public class BlackjackGameViewModel
    {
        public string Id { get; private set; }        

        public string Title { get; private set; }

        public string MinWager { get; private set; }
        public string MaxWager { get; private set; }

        public int TurnLengthInSeconds { get; private set; }
        public int WagerPeriodInSeconds { get; private set; }

        public string CurrentPlayerName { get; private set; }
        public string CurrentPlayerBalance { get; private set; }

        public bool HitButtonIsVisible { get; private set; }
        public bool StandButtonIsVisible { get; private set; }
        public bool DoubleDownButtonIsVisible { get; private set; }
        public bool WagerInputIsVisible { get; private set; }

        public bool WagerPeriodTimerIsVisible { get; private set; }
        public bool EndOfRoundTimerIsVisible { get; private set; }

        public int SecondsAwaitingPlayerAction { get; private set; }
        public int SecondsAwaitingWagers { get; private set; }        

        public BlackjackGameDealerViewModel Dealer { get; private set; }
        public IEnumerable<BlackjackGamePlayerViewModel> Players { get; private set; }        

        public BlackjackGameViewModel(MyBlackjackGame game, string currentPlayerId)
        {
            if (game == null)
                throw new ArgumentNullException("game");

            Id = game.Id;
            Title = game.Title;
            TurnLengthInSeconds = game.TurnLengthInSeconds;
            WagerPeriodInSeconds = game.BettingPeriodInSeconds;
            MinWager = game.MinWager.ToString();
            MaxWager = game.MaxWager.ToString();
            Dealer = new BlackjackGameDealerViewModel(game);
            SecondsAwaitingPlayerAction = GetTimeSpanToNowInSeconds(game.AwaitingPlayerActionSince);
            SecondsAwaitingWagers = GetTimeSpanToNowInSeconds(game.AwaitingNextRoundSince);
            Players = GetPlayers(game, SecondsAwaitingPlayerAction);
            WagerPeriodTimerIsVisible = !game.IsRoundInProgress && game.Players.Any(a => a.Wager > 0);
            EndOfRoundTimerIsVisible = game.IsRoundInProgress && game.Players.All(a => !a.HasAction);

            var currentPlayer = game.Players.FirstOrDefault(a => a.Id == currentPlayerId);
            if (currentPlayer != null)
            {
                CurrentPlayerName = currentPlayer.Alias;
                CurrentPlayerBalance = currentPlayer.Account.Balance.ToString();
                StandButtonIsVisible = currentPlayer.HasAction;
                HitButtonIsVisible = currentPlayer.HasAction && !currentPlayer.Hand.IsBlackjack;
                DoubleDownButtonIsVisible = currentPlayer.HasAction && currentPlayer.Hand.Cards.Count() == 2;
                WagerInputIsVisible = !EndOfRoundTimerIsVisible && !currentPlayer.IsLive && currentPlayer.Wager == 0;
            }            
        }

        private IEnumerable<BlackjackGamePlayerViewModel> GetPlayers(Models.MyBlackjackGame game, int secondsAwaitingPlayerAction)
        {
            var players = new List<BlackjackGamePlayerViewModel>();
            foreach (var player in game.Players)
            {
                var settlement = game.RoundInProgressSettlements
                    .FirstOrDefault(a => a.PlayerPosition == player.Position);

                var secondsAwaitingAction = player.HasAction ? secondsAwaitingPlayerAction : -1;
                players.Add(new BlackjackGamePlayerViewModel(player, secondsAwaitingAction, settlement));
            }

            return players;
        }

        private int GetTimeSpanToNowInSeconds(DateTime? starting)
        {
            if (starting == null)
                return -1;

            var timeRemaining = DateTime.UtcNow - starting.Value;
            return (int)Math.Ceiling(timeRemaining.TotalSeconds);
        }
    }
}