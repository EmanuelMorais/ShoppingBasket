namespace ShoppingBasketApi.Infrastructure.Entities
{
    public record ApplicationError(string Code, string Message)
    {
        public static ApplicationError None => new(string.Empty, string.Empty);
    }

    public static class ErrorMessages
    {
        public static readonly string InvalidRequest = "Invalid request";
    }

    public static class ErrorCode
    {
        public static readonly string GenericError = "GEN_00001";
        public static readonly string InvalidValue = "GEN_00002";
    }

}

