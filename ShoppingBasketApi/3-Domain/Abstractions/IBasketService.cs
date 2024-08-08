using ShoppingBasketApi.Application.Dtos;
using ShoppingBasketApi.Infrastructure.Helpers;

namespace ShoppingBasketApi.Domain.Abstractions;

public interface IBasketService
{
    Task<Result<ReceiptDto>> CalculateBasketTotalAsync(IEnumerable<BasketItemDto> basketItems);
    Task<Result<BasketDto>> UpdateBasketWithDiscountsAsync(IEnumerable<BasketItemDto> basketItems, bool forceRemove = false);
}