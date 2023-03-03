using Microsoft.AspNetCore.Mvc;

namespace QuizApplication.Controllers
{
    public class QuizController : Controller
    {
        public QuizController()
        {
            
        }
        
        public IActionResult Session()
        {
            return View();
        }
        
        public IActionResult Instructions()
        {
            return View();
        }
        
        public IActionResult Result()
        {
            return View();
        }
        
        public IActionResult Leaderboard()
        {
            return View();
        }
    }
}