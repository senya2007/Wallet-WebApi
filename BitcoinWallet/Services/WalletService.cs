using BitcoinWallet.Db;
using BitcoinWallet.Models;
using BitcoinWallet.Models.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BitcoinWallet.Services
{
    public class WalletService
    {
        private NetCoreEFContext Contex;

        public WalletService(NetCoreEFContext context)
        {
            Contex = context;
        }

        public List<Account> GetAllWallets(BitcoinService service)
        {
            var accountsFromDB = Contex.Accounts.ToList();
            List<Account> accountsFromBTC = service.GetAllAccounts().Select(x => new Account { Address = x.Key, Balance = x.Value }).ToList();

            var newAccounts = GetNewAccounts(accountsFromDB, accountsFromBTC);
            Contex.Accounts.AddRange(newAccounts);
            Contex.SaveChanges();

            UpdateBalanceAllAccounts(accountsFromBTC);

            return Contex.Accounts.ToList();
        }

        private IEnumerable<Account> GetNewAccounts(List<Account> accountsFromDB, List<Account> accountsFromBTC)
        {
            foreach (var account in accountsFromBTC)
            {
                if (accountsFromDB.Count == 0 || accountsFromDB.All(x => x.Address != account.Address))
                {
                    yield return account;
                }
            }
        }

        private void UpdateBalanceAllAccounts(List<Account> accountsFromBTC)
        {
            var accountsFromDB = Contex.Accounts.ToList();
            foreach (var account in accountsFromBTC)
            {
                if (accountsFromDB.Any(x => x.Address == account.Address))
                {
                    var index = accountsFromDB.IndexOf(accountsFromDB.FirstOrDefault(x => x.Address == account.Address));
                    accountsFromDB[index].Balance = account.Balance;
                }
            }

            Contex.SaveChanges();
        }

        public void UpdateBalanceFromAccount(string address, decimal balance)
        {
            var accountsFromDB = Contex.Accounts.ToList();
            var index = accountsFromDB.IndexOf(accountsFromDB.FirstOrDefault(x => x.Address == address.Replace(" ", string.Empty)));
            accountsFromDB[index].Balance = balance;

            UpdateBalanceAllAccounts(accountsFromDB);
        }

        public void UpdateBalanceFromAccount(string address, BitcoinService service)
        {
            decimal balanceFromBTC = service.GetAllAccounts().FirstOrDefault(x => x.Key == address.Replace(" ", string.Empty)).Value;
            UpdateBalanceFromAccount(address, balanceFromBTC);
        }

        public void OutputWalletTransaction(List<Transaction> list)
        {
            var outputTransactions = Contex.OutputAccountTransactions.ToList();

            if (outputTransactions.Count == 0)
            {
                Contex.OutputAccountTransactions.AddRange(list.Select(x => new OutputAccountTransaction() { Address = x.Address, Transaction = x.Blockhash }));
            }
            else
            {
                Contex.OutputAccountTransactions.AddRange(list.Skip(outputTransactions.Count).Select(x => new OutputAccountTransaction() { Address = x.Address, Transaction = x.Blockhash }));
            }
            Contex.SaveChanges();

        }

        public void InputWalletTransaction(List<Transaction> list)
        {
            var inputTransactions = Contex.InputAccountTransactions.ToList();

            if (inputTransactions.Count == 0)
            {
                Contex.InputAccountTransactions.AddRange(list.Select(x => new InputAccountTransaction() { Address = x.Address, Transaction = x.Blockhash, Confirmation = x.Confirmation }));
            }
            else
            {
                Contex.InputAccountTransactions.AddRange(list.Skip(inputTransactions.Count).Select(x => new InputAccountTransaction() { Address = x.Address, Transaction = x.Blockhash, Confirmation = x.Confirmation }));
            }
            Contex.SaveChanges();
        }
    }
}
