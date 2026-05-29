public class CreateSaleRequest
{
    public Dictionary<string, int> quantitiesByProductID { get; set; } 
    public DateOnly Date {get; set;}
    public TimeOnly Time {get; set;}
}