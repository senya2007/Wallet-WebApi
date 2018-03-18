using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BitcoinWallet.Models;
using BitcoinWallet.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BitcoinWallet.Controllers
{
    [Produces("application/json")]
    [Route("api/Bitcoin")]
    public class BitcoinController : Controller
    {
        public BitcoinService BitcoinService { get; }
        public WalletService WalletService { get; }

        public BitcoinController(BitcoinService bitcoin)
        {
            BitcoinService = bitcoin;
            WalletService = new WalletService(BitcoinService.Context);
        }

        // GET: api/Bitcoin
        [HttpGet]
        [Route("GetAllTransactions")]
        public object GetAllTransactions()
        {
            //return 1.ToString();
            return BitcoinService.GetAllTransactions().Count;
        }

        [HttpGet]
        [Route("GetLast")]
        public object GetLast()
        {
            var lastCountTransactions = BitcoinService.GetLastCountTransactions();
            var countTransactions = BitcoinService.GetBlockCount();

            var allTransactions = BitcoinService.GetAllTransactions();
            var receiveTransactions = BitcoinService.SeparateFromTypeTransactions(allTransactions)["receive"];
            if (lastCountTransactions < countTransactions)
            {
                return allTransactions.Skip(lastCountTransactions).Select(x=>new { Date = x.BlockTime, Address = x.Address, Amount = x.Amount, Confirmation = x.Confirmation});
            }
            else
            {
                return receiveTransactions.Where(x => x.Confirmation < 3).Select(x => new { Date = x.BlockTime, Address = x.Address, Amount = x.Amount, Confirmation = x.Confirmation });
            }
        }

        [HttpGet]
        [Route("GetAllAccounts")]
        public object GetAccounts()
        {
            return WalletService.GetAllWallets(BitcoinService);
        }

        [HttpGet]
        [Route("GetOutputTransactions")]
        public object GetOutputTransactions()
        {
            var allTransactions = BitcoinService.GetAllTransactions();
            var moveTransactions = BitcoinService.SeparateFromTypeTransactions(allTransactions)["move"];
            var outputTransactions = BitcoinService.SeparateFromTypeTransactions(allTransactions)["send"];
            outputTransactions.AddRange(moveTransactions);
            WalletService.OutputWalletTransaction(outputTransactions);
            return Ok();
        }

        [HttpGet]
        [Route("GetInputTransactions")]
        public object GetInputTransactions()
        {
            var allTransactions = BitcoinService.GetAllTransactions();
            var receiveTransactions = BitcoinService.SeparateFromTypeTransactions(allTransactions)["receive"];
            WalletService.OutputWalletTransaction(receiveTransactions);
            return Ok();
        }


        // GET: api/Bitcoin/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Bitcoin
        [HttpPost]
        public ActionResult Post(string address, decimal money)
        {
            if (BitcoinService.TransferBTC(BitcoinService.GetAccountWithMaximumBTC(BitcoinService.GetAllAccounts()), address, money))
            {
                WalletService.UpdateBalanceFromAccount(address, BitcoinService);
                return Ok($"Money {money} BTC send to {address} account");
            }
            else
            {
                return BadRequest($"Money {money} BTC not send to {address} account");
            }
        }

        // PUT: api/Bitcoin/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
