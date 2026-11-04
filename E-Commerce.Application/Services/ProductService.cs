using AutoMapper;
using E_Commerce.Application.Common;
using E_Commerce.Application.Contracts;
using E_Commerce.Application.DTOs.Products;
using E_Commerce.Application.Specifications;
using E_Commerce.Domain.Contracts;
using E_Commerce.Domain.Entities.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Services
{
    internal class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<Result<IReadOnlyList<BrandDto>>> GetAllBrandsAsync(CancellationToken ct = default)
        {
            var brands = await _unitOfWork.GetRepository<ProductBrand, int>().GetAllAsync(ct);
            var mappedBrands = _mapper.Map<IReadOnlyList<BrandDto>>(brands);
            return Result<IReadOnlyList<BrandDto>>.Ok(mappedBrands);
        }

        public async Task<Result<IReadOnlyList<ProductDto>>> GetAllProductsAsync(ProductQueryParams queryParams,  CancellationToken ct = default)
        {
            var specification = new ProductWithTypeAndBrandSpec(queryParams);
            var products = await _unitOfWork.GetRepository<Product, int>().GetAllAsync(specification,ct);
            var mappedProducts = _mapper.Map<IReadOnlyList<ProductDto>>(products);
            return Result<IReadOnlyList<ProductDto>>.Ok(mappedProducts);
        }

        public async Task<Result<IReadOnlyList<TypeDto>>> GetAllTypesAsync(CancellationToken ct = default)
        {
            var Types = await _unitOfWork.GetRepository<ProductType, int>().GetAllAsync(ct);
            var mappedTypes = _mapper.Map<IReadOnlyList<TypeDto>>(Types);
            return Result<IReadOnlyList<TypeDto>>.Ok(mappedTypes);
        }

        public async Task<Result<ProductDto>> GetProductByIdAsync(int id, CancellationToken ct = default)
        {
            var product = await _unitOfWork.GetRepository<Product, int>().GetByIdAsync(new ProductWithTypeAndBrandSpec(id), ct );
            if (product is null)
                return Error.NotFound("Product.NotFound", $"Product with ID {id} not found");

            return _mapper.Map<ProductDto>(product);
        }
    }
}
