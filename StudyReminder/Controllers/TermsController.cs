using Microsoft.AspNetCore.Mvc;

namespace StudyReminder.Controllers
{
    public class TermsController : Controller
    {
      public IActionResult Terms()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}
