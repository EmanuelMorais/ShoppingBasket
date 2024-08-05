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

    public async Task<Result<IEnumerable<BasketItemDto>>> UpdateBasketWithDiscountsAsync(IEnumerable<BasketItemDto> basketItems, bool forceRemove = false)
    {
        var basket = new Basket { Items = basketItems.Select(item => item.ToDomain()).ToList() };

        if (!forceRemove)
        {
            var result = await this.discountService.ApplyBasketDiscountAsync(basket);
            basket = result.Value;
        }
        else
        {
            basket.Items = basket.Items
                .Where(item => !basketItems.Any(bi => bi.ItemName == item.ItemName && bi.Quantity == 0))
                .ToList();
        }
        var updatedItems = basket.Items.Select(item => item.ToDto()).ToList();
        return Result<IEnumerable<BasketItemDto>>.Success(updatedItems);
    }
}