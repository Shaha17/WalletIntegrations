using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace ewallet_integration.Models
{
    public class Deposit
    {
        public int Id { get; set; }
        public int WalletId { get; set; }
        public DateTime DateTime { get; set; }
        public decimal Amount { get; set; }
        public string By { get; set; }
        public bool IsSuccess { get; set; }
        public string Message { get; set; }

        [ForeignKey("WalletId")] 
        [JsonIgnore]
        public virtual Wallet Wallet { get; set; }
    }
}