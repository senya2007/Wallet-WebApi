using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BitcoinWallet.Models
{
    public class Block
    {
        public string Hash { get; set; }
        public int Confirmations { get; set; }
        public int StrippedSize { get; set; }
        public int Size { get; set; }
        public int Weight { get; set; }
        public int Height { get; set; }
        public int Version { get; set; }
        public string VersionHex { get; set; }
        public IList<string> Tx { get; set; }
        public int Time { get; set; }
        public int MedianTime { get; set; }
        public int Nonce { get; set; }
        public string Bits { get; set; }
        public long Difficulty { get; set; }
        public string Chainwork { get; set; }
        public string PreviousBlockhash { get; set; }
        public string NextBlockhash { get; set; }
    }

}
