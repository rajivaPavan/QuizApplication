using Microsoft.AspNetCore.Mvc;

namespace QuizApplication.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult AccessDenied()
        {
            return RedirectToAction("Home", "Quiz");
        }
    }
}