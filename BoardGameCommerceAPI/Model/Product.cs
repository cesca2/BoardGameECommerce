public record Product
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Name { get; set; }
    public int YearPublished { get; set; }
    public float Price { get; set; }
}
