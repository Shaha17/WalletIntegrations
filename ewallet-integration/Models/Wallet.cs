using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ewallet_integration.Models
{
    public class Wallet
    {
        [Key]
        public int Id { get; set; }

        public string Msisdn { get; set; }
        public decimal Balance  { get; set; }
        public bool IsIdentified { get; set; }

        [JsonIgnore]
        public virtual ICollection<Deposit> Deposits { get; set; }
    }
}