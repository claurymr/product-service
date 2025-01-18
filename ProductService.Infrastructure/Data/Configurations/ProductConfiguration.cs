using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductService.Domain;

namespace ProductService.Infrastructure.Data.Configurations;

/// <summary>
/// Configures the entity of type <see cref="Product"/>.
/// </summary>
/// <param name="builder">The builder to be used to configure the entity.</param>
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
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(b => b.Price)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(cs => cs.Category)
            .IsRequired()
            .HasMaxLength(50);

        // Indexes
        builder.HasIndex(p => p.Category)
            .HasDatabaseName("IX_Products_Category");
        builder.HasIndex(p => p.Sku)
            .IsUnique();
    }
}
