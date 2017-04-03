using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blackjack.Domain.Tests
{
    class TestPlayer
    {
        public string Name { get; set; }
        public TestPlayerAccount Account { get; set; }
        public TestPlayer(string name)
        {
            Account = new TestPlayerAccount(10000);
            Name = name;
        }
    }
}
