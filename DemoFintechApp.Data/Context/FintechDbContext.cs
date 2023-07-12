using DemoFintechApp.Models.Entities;
using DemoFintechApp.Models.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoFintechApp.Data.Context
{
	public class FintechDbContext : IdentityDbContext<ApplicationUser>
	{
		public FintechDbContext(DbContextOptions<FintechDbContext> options)
			: base(options)
		{
			
		}
		public DbSet<Account> Accounts { get; set; }
		public DbSet<Card> Cards { get; set; }
		public DbSet<RefreshToken> RefreshTokens { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Account>()
				.Property(a => a.AccountNumber)
				.HasMaxLength(50) // Set the maximum length for the account number
				.IsRequired();   // Make the account number property required

			base.OnModelCreating(modelBuilder);
		}


	}
}
