namespace ShoppingBasketApi.Domain.Abstractions;

public interface IRulesFileProvider
{
    Task<string> GetRulesJsonAsync();
}


