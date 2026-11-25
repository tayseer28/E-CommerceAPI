using E_Commerce.Domain.Entities.Baskets;
using E_Commerce.Domain.Entities.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Specifications
{
    internal class ProductWithIdSpecefications : BaseSpecification<Product, int>
    {
        public ProductWithIdSpecefications(HashSet<int> productsIds) : base(p => productsIds.Contains(p.Id))
        {

        }
    }
}
