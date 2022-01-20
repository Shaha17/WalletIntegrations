using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ewallet_integration.Models;

namespace ewallet_integration.Context
{
    public class ContextHelper
    {
        public static async Task Seed(WalletDbContext context)
        {
            if (!context.Wallets.Any())
            {
                var mediaGenres = new List<Wallet>
                {
                    new Wallet {Balance = 95000, Msisdn = "992987019600", IsIdentified = true},
                    new Wallet {Balance = 5000, Msisdn = "992987654321", IsIdentified = false},
                    new Wallet {Balance = 0, Msisdn = "992123456789",IsIdentified = false}
                };

                context.Wallets.AddRange(mediaGenres);
                await context.SaveChangesAsync();
            }
        }
    }
}