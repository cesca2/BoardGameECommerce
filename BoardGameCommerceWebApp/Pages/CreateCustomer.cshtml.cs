using System.Security.Principal;
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
    [BindProperty] 
    public string Password {get; set;}
    public bool ValidModelEntry = true;
    public bool CustomerExists = false;


    public CreateCustomerModel(CustomersApiClient customersApi)
    {
        _customersApi = customersApi;
    }

    public  async Task<IActionResult> OnPostAsync()
    {
        CreateCustomerRequest customer = new CreateCustomerRequest
        {
            Name = Name,
            Email = Email,
            Password = Password
        };


        if (!ModelState.IsValid){
                ValidModelEntry=false;
                return Page();
            }
        
        else
        {
            var customerToken = await _customersApi.CreateCustomer(customer);
            
            GetCustomerResponse customerInfo = await _customersApi.GetCustomer(customerToken);
            HttpContext.Session.SetString("UserToken", customerToken);

            HttpContext.Session.SetString("UserName", customerInfo.Name);
            HttpContext.Session.SetString("UserEmail", customerInfo.Email);
            HttpContext.Session.SetString("UserId", customerInfo.Id.ToString());

        }

        if (HttpContext.Session.GetInt32("CheckoutRequested") == 1)
                    { return RedirectToPage("./Checkout");
                        
                    }
        else {
        return RedirectToPage("./Index");
        }
    }


}