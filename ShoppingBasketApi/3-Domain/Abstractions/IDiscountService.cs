using ShoppingBasketApi.Domain.Entities;
using ShoppingBasketApi.Infrastructure.Helpers;

namespace ShoppingBasketApi.Domain.Abstractions;

public interface IDiscountService
{
    Task<Result<Basket>> ApplyBasketDiscountAsync(Basket basket);
    Task<Result<Receipt>> ApplyDiscountsAsync(Basket basket);
}