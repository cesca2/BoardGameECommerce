using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BoardGameCommerce.Pages;
public class CreateCustomerModel : PageModel
{

    private readonly CustomersApiClient _customersApi;

    [BindProperty]         
    public string Name {get; set;}
    [BindProperty] 
    public string Email {get; set;}
    public bool ValidModelEntry = true;

    public CreateCustomerModel(CustomersApiClient customersApi)
    {
        _customersApi = customersApi;
    }

    public  async Task<IActionResult> OnPostAsync()
    {
        CreateCustomerRequest customer = new CreateCustomerRequest
        {
            Name = Name,
            Email = Email
        };

        // Check for existing customer with same email
        var customerExists = await _customersApi.GetCustomerByEmail(Email);
        if (customerExists is not null)
        {
            ValidModelEntry=false;
            return Page();
            
        }


        if (!ModelState.IsValid){
                ValidModelEntry=false;
                return Page();
            }
        
        else
        {
            await _customersApi.CreateCustomer(customer);
            var customerAPI = await _customersApi.GetCustomerByEmail(Email);
            HttpContext.Session.SetString("UserName", customerAPI.Name);
            HttpContext.Session.SetString("UserEmail", customerAPI.Email);
            HttpContext.Session.SetString("UserId", customerAPI.Id.ToString());

        }

        if (HttpContext.Session.GetInt32("CheckoutRequested") == 1)
                    { return RedirectToPage("./Checkout");
                        
                    }
        else {
        return RedirectToPage("./Index");
        }
    }


}