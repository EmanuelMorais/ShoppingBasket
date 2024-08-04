using ShoppingBasketApi.Domain.Entities;
using ShoppingBasketApi.Infrastructure.Helpers;

namespace ShoppingBasketApi.Domain.Abstractions;

public interface IReceiptService
{
    Task<Result<Receipt>> GenerateReceipt(Basket basket);
}