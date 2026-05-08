public class Product
{
    public Guid Id { get; set; } = Guid.NewGuid(); 
    public string Name { get; set; }
    public int YearPublished { get; set; }
    public float Price {get; set;}

}