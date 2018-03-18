using BitcoinWallet.Db;
using BitcoinWallet.Helper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BitcoinWallet.Models
{
    public class BitcoinService
    {
        private readonly string BitcoinCMDPath = @"E:\bitcoin\daemon\bitcoin-cli.exe";
        private readonly string RegtestString = @"-regtest";
        private const string EmptyAccountName = @"""""";

        private static int LastCountTransactions = 0;

        public NetCoreEFContext Context { get; }

        public BitcoinService(NetCoreEFContext context)
        {
            Context = context;
        }

        public Dictionary<string, List<Transaction>> SeparateFromTypeTransactions(IList<Transaction> transactions)
        {
            return transactions.GroupBy(x => x.Category).ToDictionary(x => x.Key, x => x.ToList());
        }

        public int GetLastCountTransactions()
        {
            return LastCountTransactions;
        }

        public int GetBlockCount()
        {
            LastCountTransactions =  int.Parse(ExecuteCommand(RegtestString, Commands.GetTransactionsCount.GetDescription()));
            return LastCountTransactions;
        }

        string GetBlockHash(int number)
        {
            return ExecuteCommand(RegtestString, Commands.GetBlockHash.GetDescription(), number.ToString());
        }

        Block GetBlockFromHash(string blockHash)
        {
            return JsonConvert.DeserializeObject<Block>(ExecuteCommand(RegtestString, Commands.GetBlock.GetDescription(), blockHash));
        }

        string GetRawTransaction(string txFromBlock)
        {
            return ExecuteCommand(RegtestString, Commands.GetRawTransaction.GetDescription(), txFromBlock);
        }

        DecodeRawTransaction DecodeRawTransaction(string rawTransaction)
        {
            return JsonConvert.DeserializeObject<DecodeRawTransaction>(ExecuteCommand(RegtestString, Commands.DecodeRawTransaction.GetDescription(), rawTransaction));
        }

        public Dictionary<string, decimal> GetAllAccounts()
        {
            return JsonConvert.DeserializeObject<Dictionary<string, decimal>>(ExecuteCommand(RegtestString, Commands.GetAllAccounts.GetDescription()));
        }

        bool SendToAddress(string address, decimal amount)
        {
            var transactionID = JsonConvert.DeserializeObject<string>(ExecuteCommand(RegtestString, Commands.SendMoneyToAddress.GetDescription(), address, amount.ToString(System.Globalization.CultureInfo.GetCultureInfo("en-US"))));
            return !string.IsNullOrEmpty(transactionID);
        }

        void GenerateNewAddress()
        {
            ExecuteCommand(RegtestString, Commands.GenerateNewAccount.GetDescription());
        }

        double GetReceivedByAddress(string account)
        {
            string stringBalance = ExecuteCommand(RegtestString, Commands.GetReceivedByAddress.GetDescription(), account);
            return double.Parse(stringBalance.Replace(".", ","));
        }

        public IList<Transaction> GetAllTransactions()
        {
            var count = GetBlockCount();
            return JsonConvert.DeserializeObject<List<Transaction>>(ExecuteCommand(RegtestString, Commands.GetAllTransactions.GetDescription(), count.ToString(), "0"));
        }

        public string GetAccountWithMaximumBTC(Dictionary<string, decimal> accountAmount)
        {
            var account = accountAmount.FirstOrDefault(x => x.Value == accountAmount.Max(z => z.Value)).Key.ToString();
            return account == String.Empty ? EmptyAccountName : account;
        }

        public bool TransferBTC(string fromAccount, string toAccount, decimal money)
        {
            if (IsInputAccount(toAccount))
            {
                return bool.Parse(ExecuteCommand(RegtestString, Commands.MoveMoneyToInputAccount.GetDescription(), fromAccount, toAccount, money.ToString(System.Globalization.CultureInfo.GetCultureInfo("en-US"))));
            }
            else
            {
                return SendToAddress(toAccount, money);
            }
        }

        private bool IsInputAccount(string toAccount)
        {
            var accounts = GetAllAccounts();
            return accounts.ContainsKey(toAccount.Replace(" ", string.Empty));
        }

        string ExecuteCommand(params string[] args)
        {
            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = BitcoinCMDPath,
                    Arguments = string.Join(" ", args),
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };


            var output = new StringBuilder();
            proc.OutputDataReceived += (o, e) => output.Append(e.Data);

            proc.Start();
            proc.BeginOutputReadLine();
            proc.WaitForExit(500000);

            return output.ToString();
        }
    }
}
