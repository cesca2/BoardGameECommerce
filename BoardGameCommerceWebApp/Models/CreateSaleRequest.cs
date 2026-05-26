public class CreateSaleRequest
{
    public string customer_Id { get; set; }
    public Dictionary<string, int> quantitiesByProductID { get; set; } 
}