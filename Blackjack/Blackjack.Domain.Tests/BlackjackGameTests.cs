using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Blackjack.Domain.Tests
{
    [TestClass]
    public class BlackjackGameTests
    {
        private int _numberOfPlayers = 7;

        [TestMethod]
        public void blackjack_game_can_initialize()
        {
            var game = InitializeGameForTesting();
            Assert.AreEqual(game.Players.Count(a => !a.HasAction), _numberOfPlayers);
        }

        [TestMethod]
        public void blackjack_game_can_accept_player_wagers()
        {
            var game = InitializeGameForTesting();
            SetWagers(game);

            Assert.IsTrue(game.Players.Any(a => a.Wager == 0));
            Assert.IsTrue(game.Players.Any(a => a.Wager > 0));
            Assert.IsTrue(game.Players.All(a => !a.HasAction));
        }
        
        [TestMethod]
        public void blackjack_game_can_start_new_round()
        {
            var game = InitializeGameForTesting();
            SetWagers(game);
            game.StartRound();

            Assert.IsTrue(game.IsRoundInProgress);

            var playersPlaying = game.Players.Where(a => a.IsLive);
            var playersNotPlaying = game.Players.Where(a => !a.IsLive);

            Assert.IsTrue(playersPlaying.Any() && playersPlaying.All(a => a.Hand != null && a.Wager > 0));
            Assert.IsTrue(playersNotPlaying.Any() && playersNotPlaying.All(a => a.Hand == null && a.Wager == 0 && !a.HasAction));
        }

        [TestMethod]
        public void blackjack_game_can_perform_round()
        {
            var game = InitializeGameForTesting();
            SetWagers(game);
            game.StartRound();
            PerformRound(game);

            Assert.IsTrue(game.IsRoundInProgress);
            Assert.IsTrue(game.Players.All(a => !a.HasAction));
        }

        [TestMethod]
        public void blackjack_game_can_close_round()
        {
            var game = InitializeGameForTesting();
            SetWagers(game);

            game.StartRound();

            var playersPlaying = game.Players.Count(a => a.IsLive);
            var playersNotPlaying = game.Players.Count(a => !a.IsLive);

            PerformRound(game);

            game.EndRound();

            Assert.IsFalse(game.IsRoundInProgress);
            Assert.IsTrue(game.Players.All(a => !a.IsLive && a.Hand == null && a.Wager == 0 && !a.HasAction));
        }

        private BlackjackGame InitializeGameForTesting()
        {
            var game = new BlackjackGame(
                minWager: 1, 
                maxWager: 100, 
                maxPlayers: _numberOfPlayers);

            for (int i = 0; i < _numberOfPlayers; i++)
            {
                var player = new TestPlayer(name: "p" + i);
                game.AddPlayer(player.Account, player.Name);
            }

            return game;
        }

        private void SetWagers(BlackjackGame game)
        {
            var players = game.Players.ToList();

            var playersToWager = game.Players.Where(a => a.Position % 2 == 0).ToList();
            foreach (var player in playersToWager)
            {
                player.SetWager(50);
            }
        }

        private void PerformRound(BlackjackGame game)
        {
            BlackjackGamePlayer player;
            while ((player = game.Players.FirstOrDefault(f => f.HasAction)) != null)
            {
                if (player.Hand.Score < 17)
                    player.Hit();
                else
                    player.Stand();
            }
        }
    }
}
