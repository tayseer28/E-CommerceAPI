using E_Commerce.Application.Common;
using E_Commerce.Domain.Entities.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Specifications
{
    internal class ProductWithTypeAndBrandSpec : BaseSpecification<Product, int>
    {
        // Get all products 
        public ProductWithTypeAndBrandSpec(ProductQueryParams queryParams)
                    : base(P => (!queryParams.BrandId.HasValue || P.BrandId == queryParams.BrandId.Value)
                    && (!queryParams.TypeId.HasValue || P.TypeId == queryParams.TypeId.Value)
                    && (string.IsNullOrWhiteSpace(queryParams.SearchValue) || P.Name.ToLower().Contains(queryParams.SearchValue.ToLower())))

        {
            AddInclude(p => p.ProductBrand);
            AddInclude(p => p.ProductType);
            switch(queryParams.Sort)
            {
                case ProductSortingOptions.NameAsc:
                    AddOrderBy(p => p.Name);
                    break;
                case ProductSortingOptions.NameDesc:
                    AddOrderByDescending(p => p.Name);
                    break;
                case ProductSortingOptions.PriceAsc:
                    AddOrderBy(p => p.Price);
                    break;
                case ProductSortingOptions.PriceDesc:
                    AddOrderByDescending(p => p.Price);
                    break;
                default:
                    AddOrderBy(P => P.Id);
                    break;
            }
            ApplyPagination(queryParams.PageSize, queryParams.PageIndex);
        }
        // Get product by id with type and brand
        public ProductWithTypeAndBrandSpec(int id) : base(p => p.Id == id)
        {
            AddInclude(p => p.ProductBrand);
            AddInclude(p => p.ProductType);
        }
    }
}
