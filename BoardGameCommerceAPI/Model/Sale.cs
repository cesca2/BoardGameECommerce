public class Sale
{
    public Guid Id { get; set; } = Guid.NewGuid(); 
    public string Customer_Id { get; set; }
    public List<Dictionary<string, object>> Sale_Products { get; set; }

}