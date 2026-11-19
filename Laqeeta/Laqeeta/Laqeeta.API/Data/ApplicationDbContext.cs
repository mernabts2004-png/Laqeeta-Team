using Laqeeta.API.Models;
using Microsoft.EntityFrameworkCore;
using Laqeeta.API.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Laqeeta.API.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Station> Stations { get; set; }

        public DbSet<Item> Items { get; set; }

        public DbSet<Report> Reports { get; set; }

        public DbSet<Claim> Claims { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Stations
            modelBuilder.Entity<Station>(entity =>
            {
                entity.HasKey(s => s.StationId);

                entity.Property(s => s.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasIndex(s => s.Name)
                    .IsUnique();
            });

            modelBuilder.Entity<Station>().HasData(
                new Station
                {
                    StationId = 1,
                    Name = "Station 1"
                },
                new Station
                {
                    StationId = 2,
                    Name = "Station 2"
                },
                new Station
                {
                    StationId = 3,
                    Name = "Station 3"
                }
            );

            // Items
            modelBuilder.Entity<Item>(entity =>
            {
                entity.HasKey(i => i.ItemId);

                entity.Property(i => i.Category)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(i => i.Color)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(i => i.Line)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(i => i.Direction)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(i => i.Description)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(i => i.StorageLocation)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.Property(i => i.Status)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.HasOne(i => i.FoundAtStation)
                    .WithMany(s => s.FoundItems)
                    .HasForeignKey(i => i.FoundAtStationId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(i => new
                {
                    i.Category,
                    i.Line,
                    i.FoundAt
                });

                entity.ToTable(t =>
                {
                    t.HasCheckConstraint(
                        "CK_Items_StorageLocation",
                        "[StorageLocation] IN (N'Station', N'CentralStoreroom')");

                    t.HasCheckConstraint(
                        "CK_Items_Status",
                        "[Status] IN (N'Found', N'Reserved', N'Returned')");

                    t.HasCheckConstraint(
                        "CK_Items_CentralStorageDate",
                        "([StorageLocation] = N'Station' AND [MovedToCentralAt] IS NULL) OR ([StorageLocation] = N'CentralStoreroom' AND [MovedToCentralAt] IS NOT NULL)");
                });
            });

            // Sample Found Items
            modelBuilder.Entity<Item>().HasData(
                new Item
                {
                    ItemId = 1,
                    Category = "Phone",
                    Color = "Black",
                    Line = "Line 1",
                    Direction = "Northbound",
                    FoundAt = new DateTime(2026, 10, 30, 9, 15, 0),
                    Description = "Black smartphone with a cracked screen",
                    FoundAtStationId = 1,
                    StorageLocation = "Station",
                    MovedToCentralAt = null,
                    Status = "Found"
                },
                new Item
                {
                    ItemId = 2,
                    Category = "Wallet",
                    Color = "Brown",
                    Line = "Line 1",
                    Direction = "Southbound",
                    FoundAt = new DateTime(2026, 10, 30, 12, 30, 0),
                    Description = "Brown leather wallet",
                    FoundAtStationId = 2,
                    StorageLocation = "Station",
                    MovedToCentralAt = null,
                    Status = "Found"
                },
                new Item
                {
                    ItemId = 3,
                    Category = "Bag",
                    Color = "Blue",
                    Line = "Line 1",
                    Direction = "Northbound",
                    FoundAt = new DateTime(2026, 10, 31, 14, 0, 0),
                    Description = "Small blue backpack",
                    FoundAtStationId = 3,
                    StorageLocation = "Station",
                    MovedToCentralAt = null,
                    Status = "Found"
                }
            );

            // Reports
            modelBuilder.Entity<Report>(entity =>
            {
                entity.HasKey(r => r.ReportId);

                entity.Property(r => r.PhoneNumber)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(r => r.Category)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(r => r.Color)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(r => r.Line)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(r => r.Direction)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(r => r.Description)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(r => r.SecretQuestion)
                    .HasMaxLength(200);

                entity.Property(r => r.SecretAnswerHash)
                    .HasMaxLength(500);

                entity.Property(r => r.Status)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.HasIndex(r => new
                {
                    r.Category,
                    r.Line,
                    r.LostAt
                });

                entity.ToTable(t =>
                {
                    t.HasCheckConstraint(
                        "CK_Reports_Status",
                        "[Status] IN (N'Searching', N'PossibleMatch', N'ReadyToCollect', N'Closed')");
                });
            });

            // Sample Lost Reports
            modelBuilder.Entity<Report>().HasData(
                new Report
                {
                    ReportId = 1,
                    PhoneNumber = "01000000001",
                    Category = "Phone",
                    Color = "Black",
                    Line = "Line 1",
                    Direction = "Northbound",
                    LostAt = new DateTime(2026, 10, 30, 9, 0, 0),
                    Description = "Black smartphone with a cracked screen",
                    Status = "Searching",
                    CreatedAt = new DateTime(2026, 10, 30, 10, 0, 0),
                    SecretQuestion = "What was the phone wallpaper?",
                    SecretAnswerHash = "AQAAAAIAAYagAAAAEL9Qzq3sZ4bR+FlqdBiB+saZYjE+h1GXZT2euYkgNK6cN0E4aMjvgMsaGbEM39K8+Q=="
                },
                new Report
                {
                    ReportId = 2,
                    PhoneNumber = "01000000002",
                    Category = "Wallet",
                    Color = "Brown",
                    Line = "Line 1",
                    Direction = "Southbound",
                    LostAt = new DateTime(2026, 10, 30, 12, 0, 0),
                    Description = "Brown leather wallet",
                    Status = "Searching",
                    CreatedAt = new DateTime(2026, 10, 30, 13, 0, 0),
                    SecretQuestion = "What is one item inside the wallet?",
                    SecretAnswerHash = "AQAAAAIAAYagAAAAEH69+5l/u6b5owCePp8w+rCQkGDG8YI43GxSFKoCXlMKClT1NhAss4yZjcnX+sDdtQ=="
                },
                new Report
                {
                    ReportId = 3,
                    PhoneNumber = "01000000003",
                    Category = "Bag",
                    Color = "Blue",
                    Line = "Line 1",
                    Direction = "Northbound",
                    LostAt = new DateTime(2026, 10, 31, 13, 30, 0),
                    Description = "Small blue backpack",
                    Status = "Searching",
                    CreatedAt = new DateTime(2026, 10, 31, 15, 0, 0),
                    SecretQuestion = null,
                    SecretAnswerHash = null
                }
            );

            // Claims
            modelBuilder.Entity<Claim>(entity =>
            {
                entity.HasKey(c => c.ClaimId);

                entity.Property(c => c.Status)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.HasOne(c => c.Report)
                    .WithMany(r => r.Claims)
                    .HasForeignKey(c => c.ReportId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(c => c.Item)
                    .WithMany()
                    .HasForeignKey(c => c.ItemId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Prevent duplicate claims for the same report and item.
                entity.HasIndex(c => new
                {
                    c.ReportId,
                    c.ItemId
                }).IsUnique();

                // Prevent the same item from being handed over twice.
                entity.HasIndex(c => c.ItemId)
                    .IsUnique()
                    .HasFilter("[Status] = N'Completed'");

                entity.ToTable(t =>
                {
                    t.HasCheckConstraint(
                        "CK_Claims_Status",
                        "[Status] IN (N'Pending', N'Rejected', N'Approved', N'Completed')");

                    t.HasCheckConstraint(
                        "CK_Claims_CompletedDate",
                        "([Status] = N'Completed' AND [HandedOverAt] IS NOT NULL) OR ([Status] <> N'Completed' AND [HandedOverAt] IS NULL)");
                });
            });
        }
    }
}