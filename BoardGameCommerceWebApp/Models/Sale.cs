public class Sale
{
    public Guid Id { get; set; }
    public required string Customer_Id { get; set; }
    public required Dictionary<Guid, int> QuantitiesByProductID { get; set; } // In format, Product_Id: Quantity
    public DateOnly Date { get; set; }
    public TimeOnly Time { get; set; }
}
