using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OnlineMarket.DataAccess.Repository;
using OnlineMarket.Models;
using System.Collections.Generic;

namespace OnlineMarket.DataAccess.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base (options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<AppInfo>().HasData(new List<AppInfo>
            {
                new AppInfo
                {
                    AppInfoID = 1,
                    AppName = "Applicatio Name"
                }
            });

        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<OrderHeader> OrderHeaders { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }

        public DbSet<AppInfo> AppInfos { get; set; }
        public DbSet<AppSocialAddress> AppSocialAddresses { get; set; }
        public DbSet<AppAddress> AppAddresses { get; set; }
        public DbSet<CallBack> CallBacks { get; set; }
    }
}
