using AutoMapper;
using E_Commerce.Application.Common;
using E_Commerce.Application.Contracts;
using E_Commerce.Application.DTOs.Baskets;
using E_Commerce.Domain.Contracts;
using E_Commerce.Domain.Entities.Baskets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Services
{
    internal class BasketService : IBasketService
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IMapper _mapper;

        public BasketService(IBasketRepository basketRepository, IMapper mapper)
        {
            _basketRepository = basketRepository;
            _mapper = mapper;
        }
        public async Task<Result<BasketDto>> CreateOrUpdateBasketAsync(BasketDto basket, TimeSpan? timeToIve = null, CancellationToken ct = default)
        {
            var customerBasket = _mapper.Map<BasketDto,CustomerBasket>(basket);
            var result = await _basketRepository.CreateOrUpdateBasketAsync(customerBasket, timeToIve, ct);
            return result == null ? Result<BasketDto>.Fail(Error.Failure("BasketCreate.Failure", "Cannot Create Or Update Basket"))
                : Result<BasketDto>.Ok(basket);

            
        }

        public async Task<Result<bool>> DeleteBasketAsync(string basketId, CancellationToken ct = default)
        {
            var result = await _basketRepository.DeleteBasketAsync(basketId, ct);
            return result ? Result<bool>.Ok(true) : Result<bool>.Fail(Error.Failure("BasketDelete.Failure", "Cannot Delete Basket"));
        }

        public async Task<Result<BasketDto>> GetBasketAsync(string basketId, CancellationToken ct = default)
        {
            var basket = await _basketRepository.GetBasketAsync(basketId, ct);
            if (basket is null) return Result<BasketDto>.Fail(Error.NotFound("Basket Not Found"));
            return Result<BasketDto>.Ok( _mapper.Map<CustomerBasket, BasketDto>(basket));
        }
    }
}
