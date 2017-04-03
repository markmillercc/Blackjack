using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blackjack.Domain.Tests
{
    class TestPlayerAccount : IPlayerAccount
    {
        public string Id { get; private set; }
        public double Balance { get; private set; }

        public TestPlayerAccount(double startingBalance)
        {
            Id = Guid.NewGuid().ToString();
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
