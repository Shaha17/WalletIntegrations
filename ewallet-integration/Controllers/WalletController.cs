using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ewallet_integration.Context;
using ewallet_integration.Models;
using ewallet_integration.Models.Requests;
using ewallet_integration.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace ewallet_integration.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalletController : ControllerBase
    {
        private readonly WalletDbContext _context;
        private readonly IConfiguration _configuration;

        public WalletController(WalletDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpGet("Exists")]
        public IActionResult Exists()
        {
            var msisdn = this.Request.Headers["X-UserId"].ToString();
            if (_context.Wallets.Any(w => w.Msisdn.Equals(msisdn) || w.Msisdn.Equals($"992{msisdn}")))
            {
                return Ok();
            }

            return BadRequest("Unknown UserId");
        }

        [HttpPost("AddDeposit")]
        public async Task<IActionResult> AddDeposit([FromBody] AddDepositModel data)
        {
            var msisdn = this.Request.Headers["X-UserId"].ToString();

            var wallet = _context.Wallets
                .FirstOrDefault(w => w.Msisdn.Equals(msisdn) || w.Msisdn.Equals($"992{msisdn}"));
            if (wallet == null)
            {
                return BadRequest(new Error() {Message = "Unknown UserId"});
            }

            if (data.Amount <= 0)
            {
                return BadRequest(new Error() {Message = "Invalid amount"});
            }

            var deposit = new Deposit
            {
                Amount = data.Amount,
                WalletId = wallet.Id,
                By = "test financial institution",
                DateTime = DateTime.Now,
                Message = string.Empty,
                IsSuccess = true,
            };

            if (CheckLimit(wallet, deposit))
            {
                wallet.Balance += deposit.Amount;
            }
            else
            {
                deposit.IsSuccess = false;
                deposit.Message = "Account limit exceeded";
            }

            _context.Deposits.Add(deposit);
            await _context.SaveChangesAsync();
            if (deposit.IsSuccess)
            {
                return Ok();
            }
            else
            {
                return BadRequest(new Error() {Message = deposit.Message});
            }
        }

        [NonAction]
        private static bool CheckLimit(Wallet wallet, Deposit deposit)
        {
            var balanceAfter = wallet.Balance + deposit.Amount;
            if (wallet.IsIdentified)
            {
                return balanceAfter <= 100_000;
            }
            else
            {
                return balanceAfter <= 10_000;
            }
        }

        [HttpGet("DepositsByLastMonth")]
        public async Task<IActionResult> DepositsByLastMonth()
        {
            var msisdn = this.Request.Headers["X-UserId"].ToString();

            var wallet = _context.Wallets
                .FirstOrDefault(w => w.Msisdn.Equals(msisdn) || w.Msisdn.Equals($"992{msisdn}"));
            if (wallet == null)
            {
                return BadRequest(new Error() {Message = "Unknown UserId"});
            }

            var date = DateTime.Now;
            var currMonth = new DateTime(date.Year, date.Month, 1);

            var deposits = wallet.Deposits.Where(d => d.DateTime > currMonth).Where(d => d.IsSuccess).ToList();
            var sumOfDeposits = deposits.Sum(d => d.Amount);
            return Ok(new DepositDTO()
            {
                Deposits = deposits,
                Sum = sumOfDeposits
            });
        }

        [HttpGet("GetBalance")]
        public async Task<IActionResult> GetBalance()
        {
            var msisdn = this.Request.Headers["X-UserId"].ToString();

            var wallet = _context.Wallets
                .FirstOrDefault(w => w.Msisdn.Equals(msisdn) || w.Msisdn.Equals($"992{msisdn}"));
            if (wallet == null)
            {
                return BadRequest(new Error() {Message = "Unknown UserId"});
            }

            return Ok(new {Balance = wallet.Balance});
        }

        // GET: api/WalletController2
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] {"value1", "value2"};
        }

        // GET: api/WalletController2/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/WalletController2
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/WalletController2/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/WalletController2/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}