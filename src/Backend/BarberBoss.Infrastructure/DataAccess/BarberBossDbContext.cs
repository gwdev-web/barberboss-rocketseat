using BarberBoss.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BarberBoss.Infrastructure.DataAccess;

public class BarberBossDbContext : DbContext
{
    public BarberBossDbContext(DbContextOptions<BarberBossDbContext> options) : base(options) { }

    public DbSet<Billing> Billings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Billing>(entity =>
        {
            entity.ToTable("Billings");
            entity.HasKey(billing => billing.Id);

            entity.Property(billing => billing.Id).HasColumnType("char(36)");
            entity.Property(billing => billing.Date).HasColumnType("date").IsRequired();
            entity.Property(billing => billing.BarberName).HasMaxLength(80).IsRequired();
            entity.Property(billing => billing.ClientName).HasMaxLength(120).IsRequired();
            entity.Property(billing => billing.ServiceName).HasMaxLength(120).IsRequired();
            entity.Property(billing => billing.Amount).HasColumnType("decimal(18,2)").IsRequired();
            entity.Property(billing => billing.PaymentMethod).HasConversion<int>().IsRequired();
            entity.Property(billing => billing.Status).HasConversion<int>().IsRequired();
            entity.Property(billing => billing.Notes).HasMaxLength(500);
            entity.Property(billing => billing.CreatedAt).IsRequired();
            entity.Property(billing => billing.UpdatedAt).IsRequired();

            entity.HasIndex(billing => billing.Date).HasDatabaseName("IX_Billings_Date");
            entity.HasIndex(billing => billing.Status).HasDatabaseName("IX_Billings_Status");
        });
    }
}
