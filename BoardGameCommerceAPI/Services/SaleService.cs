using System.Globalization;
using CommerceAPI.Controllers;
using Microsoft.Data.Sqlite;
using SQLitePCL;

public class SaleService : ISaleService
{
    ISaleRepository _saleRepository;

    public SaleService(ISaleRepository saleRepository)
    {
        _saleRepository = saleRepository;
    }

    public Sale? CreateSale(Sale newSale)
    {
        var rowsaffected = _saleRepository.CreateSale(newSale);
        if (rowsaffected == 0)
        {
            throw new ApplicationException("Sale could not be created succesfully");
        }

        return _saleRepository.GetSale(newSale.Id);
    }

    public Sale? GetSaleById(Guid id)
    {
        return _saleRepository.GetSale(id);
    }

    public List<Sale> GetAllSales()
    {
        var sales = _saleRepository.GetAllSales();
        if (sales is null)
            return [];

        return sales;
    }

    public List<Sale> GetSalesByCustomerId(Guid id)
    {
        var sales = _saleRepository.GetAllSales();
        if (sales is null)
            return [];

        var customer_sales = sales.Where(sale => sale.Customer_Id == id.ToString()).ToList() ?? [];
        return customer_sales;
    }
}
