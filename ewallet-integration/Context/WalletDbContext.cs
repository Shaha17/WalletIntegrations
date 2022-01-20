using ewallet_integration.Models;
using Microsoft.EntityFrameworkCore;

namespace ewallet_integration.Context
{
    public class WalletDbContext : DbContext
    {
        public WalletDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<Deposit> Deposits { get; set; }
    }
}