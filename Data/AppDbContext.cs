using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Macrofy.Models;

namespace Macrofy.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<ChefProfile> ChefProfiles => Set<ChefProfile>();
    public DbSet<MealPlan> MealPlans => Set<MealPlan>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<PartnerApplication> PartnerApplications => Set<PartnerApplication>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // ApplicationUser
        builder.Entity<ApplicationUser>()
            .HasOne(u => u.ChefProfile)
            .WithOne(c => c.User)
            .HasForeignKey<ChefProfile>(c => c.UserId);

        // Order -> Client (restrict delete)
        builder.Entity<Order>()
            .HasOne(o => o.Client)
            .WithMany(u => u.Orders)
            .HasForeignKey(o => o.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        // Order -> Chef
        builder.Entity<Order>()
            .HasOne(o => o.ChefProfile)
            .WithMany(c => c.Orders)
            .HasForeignKey(o => o.ChefProfileId)
            .OnDelete(DeleteBehavior.Restrict);

        // Review -> Order (one-to-one)
        builder.Entity<Review>()
            .HasOne(r => r.Order)
            .WithOne(o => o.Review)
            .HasForeignKey<Review>(r => r.OrderId);

        // Review -> Client
        builder.Entity<Review>()
            .HasOne(r => r.Client)
            .WithMany(u => u.Reviews)
            .HasForeignKey(r => r.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        // Review -> Chef
        builder.Entity<Review>()
            .HasOne(r => r.ChefProfile)
            .WithMany(c => c.Reviews)
            .HasForeignKey(r => r.ChefProfileId)
            .OnDelete(DeleteBehavior.Restrict);

        // Decimal precision
        builder.Entity<ChefProfile>()
            .Property(c => c.Rating).HasColumnType("decimal(3,2)");
        builder.Entity<ChefProfile>()
            .Property(c => c.HourlyRate).HasColumnType("decimal(8,2)");
        builder.Entity<MealPlan>()
            .Property(m => m.PricePerWeek).HasColumnType("decimal(8,2)");
        builder.Entity<Order>()
            .Property(o => o.TotalPrice).HasColumnType("decimal(8,2)");
        builder.Entity<ApplicationUser>()
            .Property(u => u.WeightKg).HasColumnType("decimal(5,2)");
        builder.Entity<ApplicationUser>()
            .Property(u => u.HeightCm).HasColumnType("decimal(5,2)");
    }
}
