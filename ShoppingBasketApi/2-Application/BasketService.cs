using ShoppingBasketApi.Application.Dtos;
using ShoppingBasketApi.Domain.Abstractions;
using ShoppingBasketApi.Domain.Entities;
using ShoppingBasketApi.Infrastructure.Helpers;

namespace ShoppingBasketApi.Application
{
    public class BasketService : IBasketService
    {
        private readonly IDiscountService _discountService;
        private readonly IReceiptService _receiptService;

        public BasketService(IDiscountService discountService, IReceiptService receiptService)
        {
            _discountService = discountService;
            _receiptService = receiptService;
        }

        public async Task<Result<ReceiptDto>> CalculateBasketTotalAsync(IEnumerable<BasketItemDto> basketItems)
        {
            var basket = new Basket { Items = basketItems.Select(item => item.ToDomain()).ToList() };

            var discountResult = await _discountService.ApplyDiscountsAsync(basket);
            if (!discountResult.IsSuccess)
            {
                return Result<ReceiptDto>.Failure(discountResult.Error.Code, discountResult.Error.Message);
            }

            var receiptResult = await _receiptService.GenerateReceipt(basket);
            if (!receiptResult.IsSuccess)
            {
                return Result<ReceiptDto>.Failure(receiptResult.Error.Code, receiptResult.Error.Message);
            }

            return Result<ReceiptDto>.Success(receiptResult.Value.ToDto());
        }

        public async Task<Result<IEnumerable<BasketItemDto>>> UpdateBasketWithDiscountsAsync(IEnumerable<BasketItemDto> basketItems)
        {
            var basket = new Basket { Items = basketItems.Select(item => item.ToDomain()).ToList() };

            var discountResult = await _discountService.ApplyBasketDiscountAsync(basket);

            if (!discountResult.IsSuccess)
            {
                return Result<IEnumerable<BasketItemDto>>.Failure(discountResult.Error.Code, discountResult.Error.Message);
            }

            var updatedItems = discountResult.Value.Items.Select(item => item.ToDto()).ToList();

            return Result<IEnumerable<BasketItemDto>>.Success(updatedItems);
        }
    }

}