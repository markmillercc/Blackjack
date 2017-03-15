using System;

namespace Blackjack.Domain
{
    public interface IGamblerAccount
    {
        string Id { get; }
        double Balance { get; }
        void Credit(double amount);
        void Debit(double amount);
    }
}
