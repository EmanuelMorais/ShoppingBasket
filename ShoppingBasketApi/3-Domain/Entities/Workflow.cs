namespace ShoppingBasketApi.Domain.Entities;

public class Workflow
{
    public int Id { get; set; }

    public string Name { get; set; }

    public ICollection<Rule> Rules { get; set; }
}

