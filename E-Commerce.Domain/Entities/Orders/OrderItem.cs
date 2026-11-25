using E_Commerce.Domain.Common;
using E_Commerce.Domain.Entities.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Domain.Entities.Orders
{
    public class OrderItem : BaseEntity<int>
    {
        public ProductItemOrdered ProductItemOrdered { get; set; } = default!;

        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
