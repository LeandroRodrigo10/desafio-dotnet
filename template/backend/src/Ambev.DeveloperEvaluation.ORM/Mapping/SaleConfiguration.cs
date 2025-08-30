using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Mapping
{
    public class SaleConfiguration : IEntityTypeConfiguration<Sale>
    {
        public void Configure(EntityTypeBuilder<Sale> builder)
        {
            builder.ToTable("Sales");

            builder.HasKey(s => s.Id);

            builder.Property(s => s.Number)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(s => s.Date)
                   .IsRequired();

            builder.Property(s => s.Customer)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(s => s.Branch)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(s => s.Status)
                   .IsRequired();

            builder.Property(s => s.TotalAmount)
                   .HasColumnType("decimal(18,2)");

            // Índices úteis para busca/ordenação
            builder.HasIndex(s => s.Number).IsUnique();
            builder.HasIndex(s => s.Date);
            builder.HasIndex(s => s.Customer);
            builder.HasIndex(s => s.Branch);
            builder.HasIndex(s => s.Status);

            // Relacionamento 1-N: Sale -> SaleItem
            builder.HasMany<SaleItem>("_items")
                   .WithOne()
                   .HasForeignKey("SaleId")
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class SaleItemConfiguration : IEntityTypeConfiguration<SaleItem>
    {
        public void Configure(EntityTypeBuilder<SaleItem> builder)
        {
            builder.ToTable("SaleItems");

            builder.HasKey(i => i.Id);

            builder.Property(i => i.Sku)
                   .IsRequired()
                   .HasMaxLength(64);

            builder.Property(i => i.Name)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(i => i.Quantity)
                   .IsRequired();

            builder.Property(i => i.UnitPrice)
                   .HasColumnType("decimal(18,2)");

            builder.Property(i => i.Discount)
                   .HasColumnType("decimal(18,2)");

            builder.Property(i => i.Total)
                   .HasColumnType("decimal(18,2)");

            // FK sombra criada no relacionamento acima
            builder.HasIndex("SaleId");
        }
    }
}
