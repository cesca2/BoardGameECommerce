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

    public  IActionResult OnPost()
    {
        CreateCustomerRequest customer = new CreateCustomerRequest
        {
            Name = Name,
            Email = Email
        };

        if (!ModelState.IsValid){
                ValidModelEntry=false;
                return Page();
            }
        
        else
        {
            _customersApi.CreateCustomer(customer);
            HttpContext.Session.SetString("UserName", customer.Name);

        }

        return RedirectToPage("./Index");
    }


}