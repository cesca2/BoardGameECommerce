public class Sale
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Customer_Id { get; set; }
    public Dictionary<Guid, int> QuantitiesByProductID { get; set; } // In format, Product_Id: Quantity
    public DateOnly Date { get; set; }
    public TimeOnly Time { get; set; }
}
