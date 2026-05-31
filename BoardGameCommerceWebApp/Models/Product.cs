public class Product
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public int YearPublished { get; init; }
    public float Price { get; init; }
    public int Quantity { get; set; } = 0;
}
