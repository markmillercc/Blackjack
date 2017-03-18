using Blackjack.Domain;
using Blackjack.Mvc.Models;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Blackjack.Mvc.DAL
{
    public class LiveBlackjackContext : IDisposable
    {
        private readonly IMongoCollection<LiveBlackjackGame> _LiveBlackjackGames;

        public LiveBlackjackContext()
        {
            var client = new MongoClient();

            _LiveBlackjackGames = client
                .GetDatabase("blackjack-mvc")
                .GetCollection<LiveBlackjackGame>("blackjack-games");
        }

        public LiveBlackjackGame GetGame(string id)
        {
            var game = _LiveBlackjackGames.Find(a => a.Id == id).FirstOrDefault();

            ReconstructRoundPlayerPlayerFields(game);
            RecontructPlayerGameFields(game);

            return game;
        }

        public IEnumerable<LiveBlackjackGame> GetGames(bool fullyReconstructEachGame = false)
        {
            var gameList = _LiveBlackjackGames.Find(a => true).ToList();

            if (!fullyReconstructEachGame)
                return gameList;

            foreach (var game in gameList)
            {
                ReconstructRoundPlayerPlayerFields(game);
                RecontructPlayerGameFields(game);
            }

            return gameList;
        }

        public void SaveGame(LiveBlackjackGame game)
        {
            if (game == null)
                throw new ArgumentNullException("gameRoom");

            _LiveBlackjackGames.ReplaceOne(
                filter: a => a.Id == game.Id,
                replacement: game,
                options: new UpdateOptions { IsUpsert = true });
        }

        public static void RegisterMongoDbClassMaps()
        {
            var classMapTypes = typeof(MongoDbClassMaps)
                .GetNestedTypes()
                .Where(t => t.IsSubclassOf(typeof(BsonClassMap)))
                .ToList();

            foreach (var classMapType in classMapTypes)
            {
                var classMap = (BsonClassMap)Activator.CreateInstance(classMapType);
                BsonClassMap.RegisterClassMap(classMap);
            }
        }

        private void RecontructPlayerGameFields(BlackjackGame game)
        {
            if (game == null)
                return;

            var playerGameField = typeof(BlackjackGamePlayer)
                .GetField("_game", BindingFlags.Instance | BindingFlags.NonPublic);

            game.Players.ToList()
                .ForEach(player => playerGameField.SetValue(player, game));
        }

        private void ReconstructRoundPlayerPlayerFields(BlackjackGame game)
        {
            if (game == null)
                return;

            if (!game.IsRoundInProgress)
                return;

            var gameDealerField = typeof(BlackjackGame)
                    .GetField("_dealer", BindingFlags.Instance | BindingFlags.NonPublic);

            var gameDealer = (BlackjackGameDealer)gameDealerField.GetValue(game);

            var dealerCurrentRound = gameDealer.RoundInProgress;

            foreach (var player in dealerCurrentRound.RoundPlayers)
            {
                var gamePlayer = game.Players.FirstOrDefault(a => a.Id == player.Player.Id);
                if (gamePlayer != null)
                {
                    var roundPlayerPlayerField = typeof(BlackjackGameRoundPlayer)
                        .GetProperty("Player");

                    roundPlayerPlayerField.SetValue(player, gamePlayer);
                }
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}