public class Sale
{
    public Guid Id { get; set; }
    public string Customer_Id { get; set; }
    public Dictionary<Guid, int> QuantitiesByProductID { get; set; } 

}