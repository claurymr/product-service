using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductService.Domain;

namespace ProductService.Infrastructure.Data.Configurations;

public class PriceHistoryConfiguration : IEntityTypeConfiguration<PriceHistory>
{
    public void Configure(EntityTypeBuilder<PriceHistory> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(b => b.OldPrice)
            .HasColumnType("decimal(5,2)")
            .IsRequired();

        builder.Property(b => b.OldPrice)
            .HasColumnType("decimal(5,2)")
            .IsRequired();

        builder.Property(b => b.Timestamp).IsRequired();

        builder.HasOne(b => b.Product)
            .WithMany()
            .HasForeignKey(cr => cr.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
