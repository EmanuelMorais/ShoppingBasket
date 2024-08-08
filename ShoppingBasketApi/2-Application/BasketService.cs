using ShoppingBasketApi.Application.Dtos;
using ShoppingBasketApi.Domain.Abstractions;
using ShoppingBasketApi.Domain.Entities;
using ShoppingBasketApi.Infrastructure.Helpers;

namespace ShoppingBasketApi.Application;

public class BasketService : IBasketService
{
    private readonly IDiscountService discountService;
    private readonly IReceiptService receiptService;

    public BasketService(IDiscountService discountService, IReceiptService receiptService)
    {
        this.discountService = discountService;
        this.receiptService = receiptService;
    }

    public async Task<Result<ReceiptDto>> CalculateBasketTotalAsync(IEnumerable<BasketItemDto> basketItems)
    {
        var basket = new Basket { Items = basketItems.Select(item => item.ToDomain()).ToList() };

        var basketUpdated = await this.discountService.ApplyBasketDiscountAsync(basket);

        if (!basketUpdated.IsSuccess)
        {
            return Result<ReceiptDto>.Failure(basketUpdated.Error.Code, basketUpdated.Error.Message);
        }

        var receiptResult = await this.receiptService.GenerateReceipt(basketUpdated.Value);

        if (!receiptResult.IsSuccess)
        {
            return Result<ReceiptDto>.Failure(receiptResult.Error.Code, receiptResult.Error.Message);
        }

        return Result<ReceiptDto>.Success(receiptResult.Value.ToDto());
    }

    public async Task<Result<BasketDto>> UpdateBasketWithDiscountsAsync(IEnumerable<BasketItemDto> basketItems, bool forceRemove = false)
    {
        var basket = new Basket { Items = basketItems.Select(item => item.ToDomain()).ToList() };

        if (!forceRemove)
        {
            var result = await this.discountService.ApplyBasketDiscountAsync(basket);
            basket = result.Value;
        }
        else
        {
            foreach (var item in basket.Items)
            {
                if (item.Quantity == 0 && !string.IsNullOrEmpty(item.DiscountAppliedName))
                {
                    item.ForceRemove = true;
                    item.UnitPrice = 0;
                    item.Quantity = 0;
                }
            }
        }

        return Result<BasketDto>.Success(basket.ToDto());
    }
}