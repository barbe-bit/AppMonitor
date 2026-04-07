using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;

namespace JavaMonitor.Web.Pages;

[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
[IgnoreAntiforgeryToken]
public class ErrorModel : PageModel
{
    public string? ErrorMessage { get; set; }

    public void OnGet()
    {
        ErrorMessage = HttpContext.TraceIdentifier;
    }
}
