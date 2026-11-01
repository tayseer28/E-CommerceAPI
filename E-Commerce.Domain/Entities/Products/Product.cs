using E_Commerce.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Domain.Entities.Products
{
    public class Product : BaseEntity<int>
    {
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public string PictureUrl { get; set; } = default!;
        public decimal Price { get; set; }

        #region Relationships
        public int BrandId { get; set; } // FK
        public ProductBrand ProductBrand { get; set; } = default!;

        public int TypeId { get; set; } // FK
        public ProductType ProductType { get; set; } = default!;
        #endregion
    }
}
