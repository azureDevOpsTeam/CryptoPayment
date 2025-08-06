using Application.DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.InfrastructureLayer.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Wallet> Wallets { get; set; }
    public DbSet<Payment> Payments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Wallet configuration
        modelBuilder.Entity<Wallet>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Address).IsRequired().HasMaxLength(100);
            entity.Property(e => e.PrivateKey).IsRequired().HasMaxLength(500);
            entity.Property(e => e.PublicKey).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Network).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            
            entity.HasIndex(e => e.Address).IsUnique();
            entity.HasIndex(e => new { e.Network, e.DerivationIndex }).IsUnique();
        });

        // Payment configuration
        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Amount).IsRequired().HasColumnType("decimal(18,8)");
            entity.Property(e => e.ReceivedAmount).HasColumnType("decimal(18,8)");
            entity.Property(e => e.Network).IsRequired();
            entity.Property(e => e.Status).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.ExpiresAt).IsRequired();
            entity.Property(e => e.TransactionHash).HasMaxLength(100);
            
            entity.HasOne(e => e.Wallet)
                  .WithMany(w => w.Payments)
                  .HasForeignKey(e => e.WalletId)
                  .OnDelete(DeleteBehavior.Restrict);
                  
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.ExpiresAt);
        });
    }
}