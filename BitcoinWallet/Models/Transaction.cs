using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BitcoinWallet.Models
{
    public class Transaction
    {
        public string Account { get; set; }
        public string Address { get; set; }
        public string Category { get; set; }
        public float Amount { get; set; }
        public int Vout { get; set; }
        public int Confirmation { get; set; }
        public bool Generated { get; set; }
        public string Blockhash { get; set; }
        public int Blockindex { get; set; }
        public int BlockTime { get; set; }
        public string TxId { get; set; }
        public List<string> WalletConflicts { get; set; }
        public int Time { get; set; }
        public int TimeReceived { get; set; }
    }
}
