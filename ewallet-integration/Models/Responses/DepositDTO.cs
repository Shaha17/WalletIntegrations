using System.Collections.Generic;

namespace ewallet_integration.Models.Responses
{
    public class DepositDTO
    {
        public decimal Sum { get; set; }
        public IEnumerable<Deposit> Deposits { get; set; }
    }
}