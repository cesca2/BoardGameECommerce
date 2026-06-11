using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BoardGameCommerce.Pages;

public class IndexModel : PageModel
{
    public IActionResult OnGet()
    {
        if (User.FindFirst(ClaimTypes.Role)?.Value == "Admin")
        {
            return RedirectToPage("./Admin");
        }

        return Page();
    }
}
