using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace BitcoinWallet.Helper
{
    public enum Commands
    {
        [Description("getnewaddress")]
        GenerateNewAccount,
        [Description("sendtoaddress")]
        SendMoneyToAddress,
        [Description("listaccounts")]
        GetAllAccounts,
        [Description("getblockcount")]
        GetTransactionsCount,
        [Description("getblockhash")]
        GetBlockHash,
        [Description("getblock")]
        GetBlock,
        [Description("getrawtransaction")]
        GetRawTransaction,
        [Description("decoderawtransaction")]
        DecodeRawTransaction,
        [Description("getreceivedbyaddress")]
        GetReceivedByAddress,
        [Description(@"listtransactions ""*""")]
        GetAllTransactions,
        [Description("move")]
        MoveMoneyToInputAccount
    }
}
