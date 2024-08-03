
using Microsoft.AspNetCore.Mvc;
using ShoppingBasketApi.Application.Dtos;
using ShoppingBasketApi.Domain.Abstractions;

namespace ShoppingBasketApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BasketController : ControllerBase
    {
        private readonly IBasketService _basketService;

        public BasketController(IBasketService basketService)
        {
            _basketService = basketService;
        }

        [HttpPost("calculate")]
        public async Task<IActionResult> CalculateTotal([FromBody] List<BasketItemDto> basketItems)
        {
            var result = await _basketService.CalculateBasketTotalAsync(basketItems);

            return result.Match<IActionResult>(
                receipt => Ok(new { Receipt = receipt }),
                error => BadRequest(new { error.Code, error.Message })
            );
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateBasket([FromBody] List<BasketItemDto> basketItems)
        {
            var result = await _basketService.UpdateBasketWithDiscountsAsync(basketItems);

            return result.Match<IActionResult>(
                items => Ok(items),
                error => BadRequest(new { error.Code, error.Message })
            );
        }
    }
}

