using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductService.Domain;

namespace ProductService.Infrastructure.Data.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(b => b.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(b => b.Description)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(b => b.Sku)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(b => b.Price)
            .HasColumnType("decimal(5,2)")
            .IsRequired();

        builder.Property(cs => cs.Category).IsRequired();

        builder.HasMany(b => b.PriceHistories)
            .WithOne()
            .HasForeignKey(p => p.ProductId)
            .IsRequired()
            .OnDelete(DeleteBehavior.SetNull);
    }
}
