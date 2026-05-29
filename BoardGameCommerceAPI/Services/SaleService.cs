using CommerceAPI.Controllers;
using Microsoft.Data.Sqlite;
using System.Globalization;


public class SaleService : ISaleService
{
    ISaleRepository _saleRepository;
    public SaleService(ISaleRepository saleRepository)
    {
        _saleRepository = saleRepository;

    }
    
    
    public Sale CreateSale(Sale newSale)
    {
        var rowsaffected =  _saleRepository.CreateSale(newSale);
        if (rowsaffected == 0)
        {
            throw new InvalidOperationException("Sale could not be created succesfully");
        }
        return _saleRepository.GetSale(newSale.Id);

    }

    public Sale? GetSaleById(Guid id)
    {
        return _saleRepository.GetSale(id);
        

    }


    public List<Sale> GetSalesByCustomerId(Guid id)
    {
        var sales = _saleRepository.GetAllSales();
        return sales.Where( sale => sale.Customer_Id == id.ToString() ).ToList() ?? [];
        

    }
}