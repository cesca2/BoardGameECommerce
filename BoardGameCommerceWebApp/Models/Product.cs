public class Product
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public int YearPublished { get; set; }
    public float Price { get; set; }
    public int Quantity { get; set; } = 0;
}
