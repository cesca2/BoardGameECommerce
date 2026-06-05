public record SaleDTO
{
    public required Dictionary<Guid, int> QuantitiesByProductID { get; set; } // In format, Product_Id: Quantity
    public required DateOnly Date { get; set; }
    public required TimeOnly Time { get; set; }
}
