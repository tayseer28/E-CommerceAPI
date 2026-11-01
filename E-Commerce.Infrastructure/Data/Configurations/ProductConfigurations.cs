using E_Commerce.Domain.Entities.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Infrastructure.Data.Configurations
{
    internal class ProductConfigurations : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.Property(P => P.Name)
                .HasMaxLength(100);

            builder.Property(P => P.Description)
                .HasMaxLength(500);

            builder.Property(P => P.PictureUrl)
                .HasMaxLength(200);

            builder.Property(p => p.Price)
                .HasPrecision(18, 2);

            builder.HasOne(B => B.ProductBrand)
                .WithMany()
                .HasForeignKey(P => P.BrandId);

            builder.HasOne(B => B.ProductType)
                .WithMany()
                .HasForeignKey(P => P.TypeId);
                
                
        }
    }
}
