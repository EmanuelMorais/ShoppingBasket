
using Microsoft.AspNetCore.Mvc;
using ShoppingBasketApi.Application.Dtos;
using ShoppingBasketApi.Domain.Abstractions;

namespace ShoppingBasketApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BasketController : ControllerBase
{
    private readonly IBasketService basketService;

    public BasketController(IBasketService basketService)
    {
        this.basketService = basketService;
    }

    [HttpPost("calculate")]
    public async Task<IActionResult> CalculateTotal([FromBody] List<BasketItemDto> basketItems)
    {
        var result = await this.basketService.CalculateBasketTotalAsync(basketItems);

        return result.Match<IActionResult>(
            receipt => Ok(new { Receipt = receipt }),
            error => BadRequest(new { error.Code, error.Message })
        );
    }

    [HttpPost("update")]
    public async Task<IActionResult> UpdateBasket([FromBody] List<BasketItemDto> basketItems, [FromQuery] bool forceRemove = false)
    {
        var result = await this.basketService.UpdateBasketWithDiscountsAsync(basketItems, forceRemove);

        return result.Match<IActionResult>(
            basketDto => Ok(basketDto),
            error => BadRequest(new { error.Code, error.Message })
        );
    }
}

