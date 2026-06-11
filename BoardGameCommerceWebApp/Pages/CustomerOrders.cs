using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BoardGameCommerce.Pages;

[Authorize]
public class CustomerOrdersModel : PageModel
{
    private readonly SalesApiClient _salesApi;
    private readonly ProductsApiClient _productsApi;

    [BindProperty(SupportsGet = true)]
    public Guid? OrderId { get; set; }

    public List<Product> Products { get; set; } = [];
    public List<GetSaleResponse> Sales { get; set; } = [];
    public GetSaleResponse? Sale { get; set; }
    public string InvalidPage = "";

    public CustomerOrdersModel(SalesApiClient salesApi, ProductsApiClient productsApi)
    {
        _salesApi = salesApi;
        _productsApi = productsApi;
    }

    public async Task OnGetAsync()
    {
        var token = await HttpContext.GetTokenAsync("api_token") ?? "";

        var sales = await _salesApi.GetSales(token);
        if (sales.Success)
        {
            InvalidPage = "";
            Sales = sales.Response ?? [];
        }
        else
        {
            InvalidPage = sales.Error ?? "Unidentified error";
        }
        if (OrderId is not null)
        {
            Sale = Sales.Where(x => x.Id == OrderId).ToList()[0];
            foreach (var item in Sale.quantitiesByProductID)
            {
                var productresponse = await _productsApi.GetProductAsync(item.Key);
                var product = productresponse.Response;
                if (productresponse.Success && product is not null)
                {
                    product.Quantity = item.Value;
                    Products.Add(product);
                }
            }
        }
    }
}
