using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BoardGameCommerce.Pages;

public class CustomerOrdersModel : PageModel
{

    [BindProperty(SupportsGet = true)]
    public Guid? OrderId { get; set; }
    private readonly SalesApiClient _salesApi;
         public List<Product> Products { get; set; } = [];

    private readonly ProductsApiClient _productsApi;

    public List<GetSaleResponse> Sales { get; set; } = [];
    public GetSaleResponse? Sale {get; set;}
    public CustomerOrdersModel(SalesApiClient salesApi, ProductsApiClient productsApi )
    {
        _salesApi = salesApi;
        _productsApi = productsApi;
    }

    public async Task OnGetAsync()

    {

        Console.WriteLine(OrderId);
        if (OrderId is null)
        {           
        
        Sales = await _salesApi.GetSalesByCustomerId(Guid.Parse(HttpContext.Session.GetString("UserId")));
        }
        else
        {
            Sale = await _salesApi.GetSaleById(OrderId);

        foreach (var item in Sale.quantitiesByProductID)
        {
  
            var product = await _productsApi.GetProductAsync(item.Key);
            product.Quantity = item.Value;
            Products.Add(product);

        }

        }

    }


}