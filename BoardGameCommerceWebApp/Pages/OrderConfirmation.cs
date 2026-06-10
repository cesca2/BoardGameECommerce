using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BoardGameCommerce.Pages;

public class OrderConfirmationModel : PageModel
{
    [BindProperty(SupportsGet = true)]
    public Guid? id { get; set; }

    public async Task OnGetAsync() { }
}
