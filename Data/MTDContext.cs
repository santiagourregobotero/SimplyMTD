using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

using SimplyMTD.Models.MTD;

namespace SimplyMTD.Data
{
    public partial class MTDContext : DbContext
    {
        public MTDContext()
        {
        }

        public MTDContext(DbContextOptions<MTDContext> options) : base(options)
        {
        }

        partial void OnModelBuilding(ModelBuilder builder);

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
           
			this.OnModelBuilding(builder);
        }

		public DbSet<SimplyMTD.Models.MTD.Planing> Planings { get; set; }

		public DbSet<SimplyMTD.Models.MTD.Billing> Billings { get; set; }

		public DbSet<SimplyMTD.Models.MTD.Accounting> Accountings { get; set; }

		public DbSet<SimplyMTD.Models.MTD.Accountant> Accountants { get; set; }

		public DbSet<SimplyMTD.Models.MTD.UserDetail> UserDetails { get; set; }

		public DbSet<SimplyMTD.Models.ApplicationUser> AspNetUsers { get; set; }
	}
}