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
    public class BlackjackContext : IDisposable
    {
        public readonly IMongoCollection<MyBlackjackGame> BlackjackGames;

        public BlackjackContext()
        {
            var client = new MongoClient();

            BlackjackGames = client
                .GetDatabase("blackjack-mvc")
                .GetCollection<MyBlackjackGame>("blackjack-games");
        }

        public static BlackjackContext Create()
        {
            return new BlackjackContext();
        }

        public MyBlackjackGame GetGame(string id) 
        {
            var game = BlackjackGames.Find(a => a.Id == id).FirstOrDefault();

            ReconstructRoundPlayerPlayerFields(game);
            RecontructPlayerGameFields(game);  
                               
            return game;
        }

        public IEnumerable<MyBlackjackGame> GetGameList()
        {
            return BlackjackGames.Find(a => true).ToList();
        }

        public void SaveGameRoom(MyBlackjackGame game) 
        {
            if (game == null)
                throw new ArgumentNullException("gameRoom");

            BlackjackGames.ReplaceOne(
                filter: a => a.Id == game.Id, 
                replacement: game, 
                options: new UpdateOptions { IsUpsert = true });
        }

        private void RecontructPlayerGameFields(BlackjackGame game)
        {
            if (game != null)
            {
                var playerGameField = typeof(BlackjackGamePlayer)
                    .GetField("_game", BindingFlags.Instance | BindingFlags.NonPublic);

                game.Players.ToList()
                    .ForEach(player => playerGameField.SetValue(player, game));
            }
        }

        private void ReconstructRoundPlayerPlayerFields(BlackjackGame game)
        {
            if (game?.IsRoundInProgress ?? false)
            {
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

        public void Dispose()
        {
        }
    }
}