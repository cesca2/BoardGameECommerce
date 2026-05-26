using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace BoardGameCommerce.Pages;

public class CheckoutModel : PageModel
{
    private readonly ProductsApiClient _productsApi;

     public List<Product> Products { get; set; } = [];

    [BindProperty]
    public string Basket { get; set; }
    
    public List<BasketItem> BasketItems { get; set; }
    public class BasketItem {
           
    public string productId {get; set;}
   
    public int quantity {get; set;}
    }


    public CheckoutModel(ProductsApiClient productsApi)
    {
        _productsApi = productsApi;
    }

    public IActionResult OnGet()
    {
        var  username = HttpContext.Session.GetString("UserName");
        Console.WriteLine(username);

        if(string.IsNullOrWhiteSpace(username)){return RedirectToPage("./Login");}
        else {return Page();}
    }


    public async Task OnPostBasketAsync()
    {

        BasketItems =
            JsonSerializer.Deserialize<List<BasketItem>>(Basket);        //var product = await _productsApi.GetProductAsync(BasketProductId);

        foreach (var item in BasketItems)
        {
            Console.WriteLine(item.productId);
            var product = await _productsApi.GetProductAsync(item.productId);
            product.Quantity = item.quantity;
            Products.Add(product);

        }
        
        
    }
    public async Task<IActionResult> OnPostCheckoutAsync()
    {
        return RedirectToPage("./Index");
    }
}