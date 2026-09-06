using BarberBoss.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BarberBoss.Infrastructure.DataAccess;

public class BarberBossDbContext : DbContext
{
    public BarberBossDbContext(DbContextOptions<BarberBossDbContext> options) : base(options) { }

    public DbSet<Billing> Billings { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(user => user.Id);

            entity.Property(user => user.Id).HasColumnType("char(36)");
            entity.Property(user => user.Name).HasMaxLength(100).IsRequired();
            entity.Property(user => user.Email).HasMaxLength(180).IsRequired();
            entity.Property(user => user.PasswordHash).HasMaxLength(200).IsRequired();
            entity.Property(user => user.Role).HasConversion<int>().IsRequired();
            entity.Property(user => user.CreatedAt).IsRequired();
            entity.Property(user => user.UpdatedAt).IsRequired();

            // E-mail único: a regra também é validada no caso de uso para devolver 409.
            entity.HasIndex(user => user.Email).IsUnique().HasDatabaseName("IX_Users_Email");
        });

        modelBuilder.Entity<Billing>(entity =>
        {
            entity.ToTable("Billings");
            entity.HasKey(billing => billing.Id);

            entity.Property(billing => billing.Id).HasColumnType("char(36)");
            entity.Property(billing => billing.UserId).HasColumnType("char(36)").IsRequired();
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

            entity.HasOne(billing => billing.User)
                .WithMany(user => user.Billings)
                .HasForeignKey(billing => billing.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(billing => billing.Date).HasDatabaseName("IX_Billings_Date");
            entity.HasIndex(billing => billing.Status).HasDatabaseName("IX_Billings_Status");
            entity.HasIndex(billing => billing.UserId).HasDatabaseName("IX_Billings_UserId");
        });
    }
}
