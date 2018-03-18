using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BitcoinWallet.Models.DB
{
    public class InputAccountTransaction
    {
        public int Id { get; set; }
        public string Address { get; set; }
        public string Transaction { get; set; }
        public int Confirmation { get; set; }
    }
}
