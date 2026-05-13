
public record GetProductsResponse
{
    public Guid Id { get; init; }  
    public string Name { get; init; }
    public int YearPublished { get; init; }
    public float Price {get; init;}

}