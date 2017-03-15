using Blackjack.Domain;

namespace Blackjack.Mvc.Models
{
    public class GamblerAccount : IGamblerAccount
    {
        public string Id { get; private set; }
        public double Balance { get; private set; }

        public GamblerAccount(string id, double startingBalance)
        {
            Id = id;
            Balance = startingBalance;
        }

        public void Credit(double amount)
        {
            Balance += amount;
        }

        public void Debit(double amount)
        {
            Balance -= amount;
        }
    }
}