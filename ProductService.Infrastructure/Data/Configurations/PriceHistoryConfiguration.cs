using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductService.Domain;
using ProductService.Domain.Enums;

namespace ProductService.Infrastructure.Data.Configurations;

public class PriceHistoryConfiguration : IEntityTypeConfiguration<PriceHistory>
{
    public void Configure(EntityTypeBuilder<PriceHistory> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(b => b.OldPrice)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(b => b.OldPrice)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(cs => cs.Action)
            .IsRequired()
            .HasConversion(
                v => v.ToString(),
                v => (ActionType)Enum.Parse(typeof(ActionType), v))
            .HasMaxLength(50);

        builder.Property(b => b.Timestamp)
            .IsRequired();

        builder.HasOne(b => b.Product)
            .WithMany()
            .HasForeignKey(ph => ph.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
