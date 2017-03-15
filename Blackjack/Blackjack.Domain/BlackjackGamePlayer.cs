using System;
using System.Collections.Generic;

namespace Blackjack.Domain
{
    public class BlackjackGamePlayer
    {
        public string Id { get; private set; }

        private BlackjackGame _game;

        public int Position { get; private set; }
        public IGamblerAccount Account { get; private set; }

        public string Alias { get; private set; }

        public bool IsLive { get { return _game.GetPlayerIsLive(this); } }
        public bool HasAction { get { return _game.GetPlayerHasAction(this); } }
        public double Wager { get { return _game.GetPlayerWager(this); } }
        public BlackjackHand Hand { get { return _game.GetPlayerHand(this); } }

        public BlackjackGamePlayer(IGamblerAccount account, BlackjackGame game, string alias, int position)
        {
            if (account == null)
                throw new ArgumentNullException("account", "Account is null");

            if (game == null)
                throw new ArgumentNullException("game", "Game is null");

            _game = game;            
            Alias = string.IsNullOrEmpty(alias) ? "ANON" : alias;
            Account = account;
            Position = position;
            Id = Account.Id;
        }

        public void SetWager(double amount)
        {            
            _game.PlaceWager(this, amount);
        }

        public void Hit()
        {
            _game.RequestToHit(this);
        }

        public void Stand()
        {
            _game.RequestToStand(this);
        }

        public void DoubleDown(double? forLess = null)
        {
            _game.RequestToDoubleDown(this, forLess ?? Wager);
        }
    }
}
