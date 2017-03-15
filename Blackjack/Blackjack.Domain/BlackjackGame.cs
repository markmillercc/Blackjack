using System;
using System.Collections.Generic;
using System.Linq;

namespace Blackjack.Domain
{
    public class BlackjackGame
    {        
        private BlackjackGameDealer _dealer;
        private List<BlackjackGamePlayer> _players;       
        private List<BlackjackGameRoundPlayer> _roundPlayersQueuedForNextRound;

        private BlackjackGameRound _roundInProgress { get { return _dealer.RoundInProgress; } }

        public BlackjackHand DealerHand { get { return _roundInProgress?.DealerHand; } }
        public IEnumerable<BlackjackGamePlayer> Players { get { return _players.ToList(); } }

        public int MaxPlayers { get; private set; }

        public double MinWager { get; private set; }
        public double MaxWager { get; private set; }

        public bool IsRoundInProgress { get { return _roundInProgress != null; } }
        public int PercentRemainingInDealerShoe { get { return _dealer.PercentRemainingInShoe; } }

        public IEnumerable<BlackjackHandSettlement> RoundInProgressSettlements
        {
            get
            {
                return _roundInProgress?.Settlements?.ToList() ?? 
                    Enumerable.Empty<BlackjackHandSettlement>();
            }
        }

        public BlackjackGame(double minWager, double maxWager, int maxPlayers = 6)
        {
            if (minWager <= 0)
                throw new InvalidOperationException("Min wager must be greater than 0");

            if (minWager > maxWager)
                throw new InvalidOperationException("Max wager must be greater than or equal to min wager");

            if (maxPlayers < 1)
                throw new InvalidOperationException("Game must accommodate at least 1 player");

            _dealer = new BlackjackGameDealer();
            _players = new List<BlackjackGamePlayer>(MaxPlayers);
            _roundPlayersQueuedForNextRound = new List<BlackjackGameRoundPlayer>(MaxPlayers);
            MaxPlayers = maxPlayers;
            MinWager = minWager;
            MaxWager = maxWager;
        }

        public bool IsPositionOpen(int position)
        {
            return position > 0 && position <= MaxPlayers &&
                !_players.Any(a => a.Position == position);
        }

        public void AddPlayer(IGamblerAccount account, string alias, int position)
        {
            if (account == null)
                throw new ArgumentNullException("account", "Account is null");

            if (_players.Count() >= MaxPlayers)
                throw new InvalidOperationException("Game is full");

            if (!IsPositionOpen(position))
                throw new InvalidOperationException("Position is not valid");

            if (account.Balance < MinWager)
                throw new InvalidOperationException("Insufficient player funds");            

            _players.Add(new BlackjackGamePlayer(account, this, alias, position));
        }

        public void AddPlayer(IGamblerAccount account, string alias)
        {
            int position = 1;
            while (position <= MaxPlayers && !IsPositionOpen(position))
                position++;

            AddPlayer(account, alias, position);
        }

        public void RemovePlayer(BlackjackGamePlayer player)
        {
            if (_players.Contains(player))
            {
                if (player.IsLive)
                    throw new InvalidOperationException("Player is in live round");

                _players.Remove(player);
                _roundPlayersQueuedForNextRound
                    .Remove(_roundPlayersQueuedForNextRound
                        .FirstOrDefault(a => a.Player.Id == player.Id));
            }
        }

        public void RefreshDealerShoe()
        {
            _dealer.RefreshShoe();
        }

        public virtual void StartRound()
        {
            if (IsRoundInProgress)
                throw new InvalidOperationException("Live round in progress");

            if (!_roundPlayersQueuedForNextRound.Any())
                throw new InvalidOperationException("No players have wagered");

            var roundInProgress = new BlackjackGameRound(_roundPlayersQueuedForNextRound);
            _roundPlayersQueuedForNextRound.Clear();

            _dealer.Deal(roundInProgress);            
        }

        public BlackjackHandSettlement SettlePlayerHand(BlackjackGamePlayer player)
        {
            return _dealer.SettleHand(player);
        }

        public virtual void EndRound()
        {            
            _dealer.CloseRound();
        }

        internal void PlaceWager(BlackjackGamePlayer player, double amount)
        {
            if (!_players.Contains(player))
                throw new InvalidOperationException("'player' is null or invalid");

            if (player.IsLive)
                throw new InvalidOperationException("Player is in live round");

            if (_roundPlayersQueuedForNextRound.Any(a => a.Player.Id == player.Id))
                throw new InvalidOperationException();

            if (amount > player.Account.Balance)
                throw new InvalidOperationException("Insufficient funds");

            if (amount < MinWager || amount > MaxWager)
                throw new InvalidOperationException("Player wager is out of range");

            player.Account.Debit(amount);
            _roundPlayersQueuedForNextRound.Add(new BlackjackGameRoundPlayer(player, amount));            
        }

        internal void RequestToHit(BlackjackGamePlayer player)
        {
            _dealer.HandleRequestToHit(player);
        }

        internal void RequestToStand(BlackjackGamePlayer player)
        {
            _dealer.HandleRequestToStand(player);
        }

        internal void RequestToDoubleDown(BlackjackGamePlayer player, double amount)
        {
            if (amount > player?.Account?.Balance)
                throw new InvalidOperationException("Insufficient funds");

            _dealer.HandleRequestToDoubleDown(player, amount);
        }

        internal bool GetPlayerIsLive(BlackjackGamePlayer player)
        {
            return _roundInProgress?.GetRoundPlayer(player) != null;
        }

        internal BlackjackHand GetPlayerHand(BlackjackGamePlayer player)
        {
            return _roundInProgress?.GetRoundPlayer(player)?.Hand;
        }

        internal double GetPlayerWager(BlackjackGamePlayer player)
        {
            return _roundInProgress?.GetRoundPlayer(player)?.Wager
                ?? _roundPlayersQueuedForNextRound.SingleOrDefault(a => a.Player.Id == player.Id)?.Wager
                ?? 0;
        }

        internal bool GetPlayerHasAction(BlackjackGamePlayer player)
        {
            return _roundInProgress?.GetRoundPlayer(player)?.HasAction ?? false;
        }        
    }
}
