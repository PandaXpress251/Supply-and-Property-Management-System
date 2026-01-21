using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SIETE.Models;
using SIETE.Models.Supply;
using SIETE.Models.Property;

namespace SIETE.Data
{
    public class SIETEContext : DbContext
    {
        public SIETEContext (DbContextOptions<SIETEContext> options)
            : base(options)
        {
        }

        public DbSet<SIETE.Models.Property.Property> Property { get; set; } = default!;
        public DbSet<SIETE.Models.PriceThreshold> PriceThreshold { get; set; } = default!;

        public DbSet<SIETE.Models.FundCluster> FundCluster { get; set; } = default!;
        public DbSet<SIETE.Models.Supplier> Supplier { get; set; } = default!;
        public DbSet<SIETE.Models.Employee> Employee { get; set; } = default!;
        public DbSet<SIETE.Models.PlantillaPosition> PlantillaPosition { get; set; } = default!;
        public DbSet<SIETE.Models.Office> Office { get; set; } = default!;
        public DbSet<SIETE.Models.UserAccount> UserAccount { get; set; } = default!;


        public DbSet<SIETE.Models.Supply.Supply> Supply { get; set; } = default!;
        public DbSet<SIETE.Models.Supply.SupplyTransaction> SupplyTransaction { get; set; } = default!;
        public DbSet<SIETE.Models.Supply.SupplyInDetail> SupplyInDetail { get; set; } = default!;
        public DbSet<SIETE.Models.Supply.SupplyOutDetail> SupplyOutDetail { get; set; } = default!;
        public DbSet<SIETE.Models.Supply.StockCard> StockCard { get; set; } = default!;
        public DbSet<SIETE.Models.Property.PropertyTransaction> PropertyTransaction { get; set; } = default!;
        public DbSet<SIETE.Models.Property.PropertyTransactionDetail> PropertyTransactionDetail { get; set; } = default!;
        public DbSet<SIETE.Models.Property.PropertyAssignment> PropertyAssignment { get; set; } = default!;
        public DbSet<SIETE.Models.Property.PropertyAssignmentHistory> PropertyAssignmentHistory { get; set; } = default!;
        public DbSet<SIETE.Models.Property.PropertyCard> PropertyCard { get; set; } = default!;


        // ✅ Detects if thresholds are modified and updates Supply categories
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            bool thresholdUpdated = ChangeTracker.Entries<PriceThreshold>()
                                    .Any(e => e.State == EntityState.Modified || e.State == EntityState.Added);

            var result = await base.SaveChangesAsync(cancellationToken);

            if (thresholdUpdated)
            {
                await UpdateAllSupplyCategories();
            }

            return result;
        }

        // ✅ Updates all supply categories based on the latest thresholds
        private async Task UpdateAllSupplyCategories()
        {
            var threshold = await GetLatestThreshold();

            if (threshold != null)
            {
                var properties = await Property.ToListAsync();

                foreach (var property in properties)
                {
                    property.UpdateCategory(threshold);
                }

                await SaveChangesAsync();
            }
        }

        // ✅ Retrieves the latest threshold values
        private async Task<PriceThreshold?> GetLatestThreshold()
        {
            return await PriceThreshold
                .OrderByDescending(t => t.EffectiveDate)
                .FirstOrDefaultAsync();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

          
            //Hard Coding for Plantilla Position
            modelBuilder.Entity<PlantillaPosition>().HasData(
                new PlantillaPosition
                {
                    PositionID = 1,
                    PositionTitle = "Programmer",
                    IsActive = true
                }
               
            );

            modelBuilder.Entity<Office>().HasData(
                new Office
                {
                    OfficeID = 1,
                    OfficeName = "Admin Office",
                    Acronym = "Ad",
                    OfficeType = "Office",
                    RespCenter_Code = "Admin",
                    Parent_Code = "Admin"
                }
               
            );

            modelBuilder.Entity<Employee>().HasData(
                new Employee
                {
                    EmployeeID = 1,
                    FirstName = "Ernest",
                    EmailAddress = "niere@gmail.com",
                    PhoneNumber = "09123456789",
                    LastName = "Niere",
                    Title = "Mr.",
                    Suffix = "Jr.",
                    OfficeID = 1,
                    PositionID = 1,
                    IsAccountablePerson = true,
                    IsActive = true
                }
            );

            modelBuilder.Entity<UserAccount>().HasData(
                new UserAccount
                {
                    UserID = 1,
                    EmployeeID = 1,
                    Username = "Admin",
                    Password = "Admin123",
                    Role = Roles.Admin
                }
            );
            


            modelBuilder.Entity<PriceThreshold>().HasData(

             new PriceThreshold
             {
                 Id = 1,
                 PropertyThreshold = 50000,
                 SPThreshold = 15000,
                 EffectiveDate = DateTime.Now
             }
          );


            modelBuilder.Entity<FundCluster>().HasData(

                  new FundCluster
                  {
                      FundClusterID = 1,
                      FundClusterCode = "F101",
                      FundClusterName = "MDS"
                  },

                  new FundCluster
                  {
                      FundClusterID = 2,
                      FundClusterCode = "F102",
                      FundClusterName = "TRUST RECEIPTS"
                  },

                  new FundCluster
                  {
                      FundClusterID = 3,
                      FundClusterCode = "F103",
                      FundClusterName = "TRUST TRAINING"
                  },
                  new FundCluster
                  {
                      FundClusterID = 4,
                      FundClusterCode = "F104",
                      FundClusterName = "CFAG"
                  }
            );

            // Configure Employee-PlantillaPosition relationship
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.PlantillaPosition)
                .WithMany(p => p.Employees)
                .HasForeignKey(e => e.PositionID)
                .OnDelete(DeleteBehavior.SetNull);

            // Configure Employee-Office relationship
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Office)
                .WithMany(o => o.Employees)
                .HasForeignKey(e => e.OfficeID)
                .OnDelete(DeleteBehavior.SetNull);

        }
    
    }
}
