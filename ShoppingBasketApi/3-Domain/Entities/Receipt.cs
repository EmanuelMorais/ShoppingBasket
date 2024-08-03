using ShoppingBasketApi.Application.Dtos;

namespace ShoppingBasketApi.Domain.Entities
{
    public class Receipt
    {
        public Guid BasketId { get; set; }

        public decimal DiscountsApplied { get; set; }

        public decimal TotalPrice { get; set; }

        public ReceiptDto ToDto()
        {
            return new ReceiptDto
            {
                BasketId = BasketId,
                DiscountsApplied = DiscountsApplied,
                TotalPrice = TotalPrice
            };
        }
    }
}