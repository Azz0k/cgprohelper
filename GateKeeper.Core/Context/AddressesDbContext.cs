using GateKeeper.Core.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GateKeeper.Core.Context
{
    public class AddressesDbContext : DbContext
    {
        public DbSet<ForeingEmails> foreingAddresses { get; set; }
        public DbSet<LocalMonitoredEmails> localMonitoredAddresses { get; set; }
        public DbSet<AllowedDomains> allowedDomains { get; set; }
        public DbSet<AllowedEmails> allowedEmails { get; set; }
        public DbSet<User> Users { get; set; }
        private string fileName = "AddressDatabase.sqlite";
        public AddressesDbContext()
        {
        }
        public AddressesDbContext(DbContextOptions<AddressesDbContext> options)
        : base(options)
        {
        }
        protected override void ConfigureConventions(
            ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Properties<string>().UseCollation("NOCASE");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite($"Data Source={fileName}");
            }
            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ForeingEmails>(entity =>
            {
                entity.ToTable("ForeingAddresses");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.ReceivedDate).IsRequired();
            });
            modelBuilder.Entity<LocalMonitoredEmails>(entity =>
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
            modelBuilder.Entity<AllowedEmails>(entity =>
            {
                entity.ToTable($"{nameof(AllowedEmails)}");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Email).IsUnique();
            });
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e=>e.UserName).IsUnique();
                entity.Property(e=>e.Enabled).IsRequired().HasDefaultValue(true);
                entity.Property(e=>e.Hash).IsRequired().UseCollation("BINARY");
                entity.Property(e=>e.FullName).IsRequired();
                entity.Property(e => e.TokenVersion).IsRequired().HasDefaultValue(Int32.MinValue);
                entity.Property(e => e.IsAdmin).IsRequired().HasDefaultValue(false);
            });
            base.OnModelCreating(modelBuilder);
        }
    }
}
