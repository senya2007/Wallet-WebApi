using BitcoinWallet.Models;
using BitcoinWallet.Models.DB;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BitcoinWallet.Db
{
    public class NetCoreEFContext : DbContext
    {
        public NetCoreEFContext(DbContextOptions<NetCoreEFContext> options)
            :base(options)
        {

        }

        public NetCoreEFContext()
        {
        }

        public static string ConnectionString { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(ConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }

        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<InputAccountTransaction> InputAccountTransactions { get; set; }
        public virtual DbSet<OutputAccountTransaction> OutputAccountTransactions { get; set; }
    }
}
