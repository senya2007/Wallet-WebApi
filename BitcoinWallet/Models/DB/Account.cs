using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BitcoinWallet.Models.DB
{
    public class Account
    {
        public int Id { get; set; }
        public string Address { get; set; }
        public string HashPassword { get; set; }
        public decimal Balance { get; set; }
    }
}
