using Blackjack.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Blackjack.Mvc.Models
{
    public class MyBlackjackGame : BlackjackGame
    {
        public string Id { get; private set; }

        public string Title { get; set; }

        public int TurnLengthInSeconds { get; private set;  }

        public int BettingPeriodInSeconds { get; private set; }

        public DateTime? AwaitingPlayerActionSince { get; set; }

        public DateTime? AwaitingNextRoundSince { get; set; }

        private List<string> _playerIdsFromMissedRounds;                     

        public bool PlayerActionIsExpired
        {
            get
            {
                return AwaitingPlayerActionSince.HasValue && 
                    DateTime.UtcNow > AwaitingPlayerActionSince.Value.AddSeconds(TurnLengthInSeconds);
            }
        }

        public MyBlackjackGame(double minWager, double maxWager, int turnLengthInSeconds, int bettingPeriodInSeconds) 
            : base(minWager, maxWager)
        {
            Id = Guid.NewGuid().ToString().Replace("-", "");
            _playerIdsFromMissedRounds = new List<string>();
            TurnLengthInSeconds = turnLengthInSeconds;
            BettingPeriodInSeconds = bettingPeriodInSeconds;
        }

        public void AddPlayerToMissedRoundList(BlackjackGamePlayer player)
        {
            if (Players.Contains(player))
                _playerIdsFromMissedRounds.Add(player.Id);
        }

        public void RemovePlayerFromMissedRoundList(BlackjackGamePlayer player)
        {
            _playerIdsFromMissedRounds.RemoveAll(id => id == player.Id);
        }

        public void RemoveStagnantPlayers()
        {
            _playerIdsFromMissedRounds
                .GroupBy(id => id)
                .Select(a => new { playerId = a.Key, missedRounds = a.Count() })
                .Where(a => a.missedRounds > 5)
                .Select(a => a.playerId)
                .ToList()
                .ForEach(playerId =>
                {
                    var player = Players.FirstOrDefault(a => a.Id == playerId && !a.IsLive);
                    RemovePlayer(player);
                    RemovePlayerFromMissedRoundList(player);
                });
        }

        public override void StartRound()
        {
            if (PercentRemainingInDealerShoe < 20)
                RefreshDealerShoe();

            base.StartRound();

            foreach (var player in Players)
            {
                if (player.IsLive)
                    RemovePlayerFromMissedRoundList(player);
                else
                    AddPlayerToMissedRoundList(player);
            }

            RemoveStagnantPlayers();

            if (Players.Any(a => a.HasAction))
            {
                AwaitingPlayerActionSince = DateTime.Now;
            }
            else
            {
                SettlePlayers();
                AwaitingPlayerActionSince = null;
            }

            AwaitingNextRoundSince = null;
        }

        public override void EndRound()
        {
            base.EndRound();
            if (AwaitingNextRoundSince.HasValue)
                AwaitingNextRoundSince = DateTime.UtcNow;
        }

        public void ForceCurrentActionToStand()
        {
            var currentAction = Players.FirstOrDefault(a => a.HasAction);

            if (currentAction == null)
            {
                AwaitingPlayerActionSince = null;
            }
            else if (PlayerActionIsExpired)
            {
                currentAction.Stand();

                if (Players.Any(a => a.HasAction))
                {
                    AwaitingPlayerActionSince = DateTime.UtcNow;
                }
                else
                {
                    SettlePlayers();
                    AwaitingPlayerActionSince = null;
                }
            }
        }

        public void PlayerWagerRequest(BlackjackGamePlayer player, double wager)
        {
            player.SetWager(wager);

            if (!IsRoundInProgress && Players.Count() == 1)
            {
                StartRound();
                return;
            }

            if (!AwaitingNextRoundSince.HasValue)
                AwaitingNextRoundSince = DateTime.UtcNow;
        }

        public void PlayerActionRequest(BlackjackGamePlayer player, string request)
        {
            var currentAction = Players.FirstOrDefault(a => a.HasAction);

            if (PlayerActionIsExpired)
            {
                player.Stand();
            }
            else
            {
                switch (request.ToLower())
                {
                    case "hit": player.Hit(); break;

                    case "doubledown": player.DoubleDown(); break;

                    case "stand": player.Stand(); break;
                }
            }

            var nextAction = Players.FirstOrDefault(a => a.HasAction);
            if (nextAction == null)
            {
                SettlePlayers();
                AwaitingPlayerActionSince = null;
            }
            else if (nextAction.Id != currentAction.Id)
            {
                AwaitingPlayerActionSince = DateTime.UtcNow;
            }
        }

        private void SettlePlayers()
        {
            Players
                .Where(a => a.IsLive)
                .ToList()
                .ForEach(a => SettlePlayerHand(a));
        }
    }
}