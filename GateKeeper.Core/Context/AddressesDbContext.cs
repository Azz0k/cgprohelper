using GateKeeper.Core.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GateKeeper.Core.Context
{
    public class AddressesDbContext : DbContext
    {
        public DbSet<ForeingAddresses> foreingAddresses { get; set; }
        public DbSet<LocalMonitoredAddresses> localMonitoredAddresses { get; set; }
        public DbSet<AllowedDomains> allowedDomains { get; set; }
        private string fileName;
        public AddressesDbContext()
        {
            this.fileName = "AddressDatabase.sqlite";
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source={fileName}");
            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ForeingAddresses>(entity =>
            {
                entity.ToTable("ForeingAddresses");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.ReceivedDate).IsRequired();
            });
            modelBuilder.Entity<LocalMonitoredAddresses>(entity =>
            {
                entity.ToTable("localMonitoredAddresses");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.IsReplyAllowed).IsRequired().HasDefaultValue(false);
            });
            modelBuilder.Entity<AllowedDomains>(entity => 
            {
                entity.ToTable($"{nameof(AllowedDomains)}");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Domain).IsUnique();
            });
            base.OnModelCreating(modelBuilder);
        }
    }
}
