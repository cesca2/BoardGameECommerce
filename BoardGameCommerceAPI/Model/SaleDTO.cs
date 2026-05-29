public class SaleDTO
{
    public Dictionary<Guid, int> QuantitiesByProductID { get; set; } // In format, Product_Id: Quantity
    public DateOnly Date {get; set;}
    public TimeOnly Time {get; set;}
}