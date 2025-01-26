using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;

namespace PromoCodeFactory.DataAccess.Data
{
    public class DataContext : DbContext
    {
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Preference> Preferences { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<PromoCode> PromoCodes { get; set; }
        public DbSet<CustomerPreference> CustomerPreferences { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CustomerPreference>()
                .HasKey(cp => new { cp.CustomerId, cp.PreferenceId });

            modelBuilder.Entity<CustomerPreference>()
                .HasOne(cp => cp.Customer)
                .WithMany(c => c.CustomerPreferences)
                .HasForeignKey(cp => cp.CustomerId);

            modelBuilder.Entity<CustomerPreference>()
                .HasOne(cp => cp.Preference)
                .WithMany()
                .HasForeignKey(cp => cp.PreferenceId);

            modelBuilder.Entity<PromoCode>()
                .HasOne(p => p.Customer)
                .WithMany(c => c.PromoCodes)
                .HasForeignKey(p => p.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Employee>()
                .Property(e => e.FirstName).HasMaxLength(100);
            modelBuilder.Entity<Employee>()
                .Property(e => e.LastName).HasMaxLength(100);
            modelBuilder.Entity<Employee>()
                .Property(e => e.Email).HasMaxLength(100);

            modelBuilder.Entity<Role>()
                .Property(r => r.Name).HasMaxLength(50);
            modelBuilder.Entity<Role>()
                .Property(r => r.Description).HasMaxLength(200);

            modelBuilder.Entity<Customer>()
                .Property(c => c.FirstName).HasMaxLength(100);
            modelBuilder.Entity<Customer>()
                .Property(c => c.LastName).HasMaxLength(100);
            modelBuilder.Entity<Customer>()
                .Property(c => c.Email).HasMaxLength(100);

            modelBuilder.Entity<PromoCode>()
                .Property(p => p.Code).HasMaxLength(50);
            modelBuilder.Entity<PromoCode>()
                .Property(p => p.ServiceInfo).HasMaxLength(200);
            modelBuilder.Entity<PromoCode>()
                .Property(p => p.PartnerName).HasMaxLength(100);

            modelBuilder.Entity<Preference>()
                .Property(p => p.Name).HasMaxLength(100);
        }
    }
}