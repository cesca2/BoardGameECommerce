using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BoardGameCommerce.Pages;

public class CustomerOrdersModel : PageModel
{
    private readonly SalesApiClient _salesApi;
    private readonly ProductsApiClient _productsApi;

    [BindProperty(SupportsGet = true)]
    public Guid? OrderId { get; set; }

    public List<Product> Products { get; set; } = [];
    public List<GetSaleResponse> Sales { get; set; } = [];
    public GetSaleResponse? Sale { get; set; }

    public CustomerOrdersModel(SalesApiClient salesApi, ProductsApiClient productsApi)
    {
        _salesApi = salesApi;
        _productsApi = productsApi;
    }

    public async Task OnGetAsync()
    {
        var token = HttpContext.Session.GetString("UserToken");

        Sales = await _salesApi.GetSales(token);
        if (OrderId is not null)
        {
            Sale = Sales.Where(x => x.Id == OrderId).ToList()[0];
            foreach (var item in Sale.quantitiesByProductID)
            {
                var product = await _productsApi.GetProductAsync(item.Key);
                product.Quantity = item.Value;
                Products.Add(product);
            }
        }
    }
}
