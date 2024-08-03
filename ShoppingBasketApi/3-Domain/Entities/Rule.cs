namespace ShoppingBasketApi.Domain.Entities
{
    public class Rule
    {
        public int Id { get; set; }

        public string RuleName { get; set; }

        public string SuccessEvent { get; set; }

        public string ErrorMessage { get; set; }

        public string Expression { get; set; }

        public string RuleExpressionType { get; set; }

        public decimal? DiscountPercentage { get; set; }

        public decimal? DiscountAmount { get; set; }

        public ErrorType ErrorType { get; set; }

        public int WorkflowId { get; set; }

        public Workflow Workflow { get; set; }
    }

    public enum ErrorType
    {
        Warning,
        Error,
        Critical
    }
}
