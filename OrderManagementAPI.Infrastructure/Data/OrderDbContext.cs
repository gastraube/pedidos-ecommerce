using Microsoft.EntityFrameworkCore;
using OrderManagementAPI.Domain.Entities;
using OrderManagementAPI.Domain.Enums;

namespace OrderManagementAPI.Infrastructure.Data;

public class OrderDbContext : DbContext
{
    public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options)
    {
    }

    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .ValueGeneratedNever();

            entity.Property(e => e.CustomerId)
                .IsRequired();

            entity.Property(e => e.Status)
                .HasConversion<int>();

            entity.Property(e => e.CreatedAt)
                .IsRequired();

            entity.HasMany(e => e.Items)
                .WithOne()
                .HasForeignKey(oi => oi.OrderId);
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .ValueGeneratedNever();

            entity.Property(e => e.OrderId)
                .IsRequired();

            entity.Property(e => e.ProductName)
                .HasMaxLength(500)
                .IsRequired();

            entity.Property(e => e.Quantity)
                .IsRequired();

            entity.Property(e => e.UnitPrice)
                .HasPrecision(18, 2)
                .IsRequired();
        });
    }
}