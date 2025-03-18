using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DrawTogether.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public string UserId { get; set; }

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            if (Request.Cookies["UserId"] == null)
            {
                UserId = Guid.NewGuid().ToString();
                Response.Cookies.Append("UserId", UserId, new CookieOptions { Expires = DateTimeOffset.UtcNow.AddDays(30) }); ;
            }
            else
            {
                UserId = Request.Cookies["UserId"];
            }
        }
    }
}
