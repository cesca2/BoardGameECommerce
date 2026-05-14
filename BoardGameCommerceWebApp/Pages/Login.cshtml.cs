using System.Net;
using System.Runtime.InteropServices.Marshalling;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;

namespace BoardGameCommerce.Pages;
public class LoginModel : PageModel
{

    private readonly CustomersApiClient _customersApi;

    [BindProperty] 
    public string Email {get; set;}
    public bool ValidModelEntry = true;

    public GetCustomerResponse loggedCustomer = null; 

    public LoginModel(CustomersApiClient customersApi)
    {
        _customersApi = customersApi;
    }


    public  async Task<IActionResult> OnPostAsync()
    {
        GetCustomerResponse? customer = await _customersApi.GetCustomerByEmail(Email);

    

        RedirectToPage("./Index");
        
        if (customer is not null)
            {
                HttpContext.Session.SetString("UserName", customer.Name);
                return RedirectToPage("./Index");}
        else
        {
            ValidModelEntry=false;

            return Page();
        }
    }





}