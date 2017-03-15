using Blackjack.Domain;
using Blackjack.Mvc.Models;
using MongoDB.Bson.Serialization;

namespace Blackjack.Mvc.DAL
{
    public class MongoDbClassMaps
    {
        public class AccountClassMap : BsonClassMap<GamblerAccount>
        {
            public AccountClassMap()
            {
                AutoMap();
            }
        }

        public class BlackjackGameClassMap : BsonClassMap<BlackjackGame>
        {
            public BlackjackGameClassMap()
            {
                AutoMap();
                MapCreator(a => new BlackjackGame(a.MinWager, a.MaxWager, a.MaxPlayers));
                MapField("_roundPlayersQueuedForNextRound");
                MapField("_dealer");
                MapField("_players");
            }
        }

        public class BlackjackGameDealerClassMap : BsonClassMap<BlackjackGameDealer>
        {
            public BlackjackGameDealerClassMap()
            {
                AutoMap();
                MapField("_shoe");
            }
        }

        public class BlackjackGamePlayerClassMap : BsonClassMap<BlackjackGamePlayer>
        {
            public BlackjackGamePlayerClassMap()
            {
                AutoMap();
            }
        }

        public class BlackjackGameRoundClassMap : BsonClassMap<BlackjackGameRound>
        {
            public BlackjackGameRoundClassMap()
            {
                AutoMap();
                MapField("_roundPlayers");
                MapProperty("RoundPlayers");
                MapField("_dealerCards");
                MapField("_settlements");
            }
        }

        public class BlackjackGameRoundPlayerClassMap : BsonClassMap<BlackjackGameRoundPlayer>
        {
            public BlackjackGameRoundPlayerClassMap()
            {
                AutoMap();
                MapField("_cards");
            }
        }

        public class BlackjackHandClassMap : BsonClassMap<BlackjackHand>
        {
            public BlackjackHandClassMap()
            {
                AutoMap();
            }
        }

        public class BlackjackHandSettlementClassMap : BsonClassMap<BlackjackHandSettlement>
        {
            public BlackjackHandSettlementClassMap()
            {
                AutoMap();
            }
        }

        public class CardClassMap : BsonClassMap<Card>
        {
            public CardClassMap()
            {
                AutoMap();
            }
        }

        public class CardDeckClassMap : BsonClassMap<CardDeck>
        {
            public CardDeckClassMap()
            {
                AutoMap();
                MapField("_cards");
            }
        }

        public class CardShoeClassMap : BsonClassMap<CardShoe>
        {
            public CardShoeClassMap()
            {
                AutoMap();
            }
        }

        public class MyBlackjackGameClassMap : BsonClassMap<MyBlackjackGame>
        {
            public MyBlackjackGameClassMap()
            {
                AutoMap();
                MapField("_playerIdsFromMissedRounds");
            }
        }
    }
}