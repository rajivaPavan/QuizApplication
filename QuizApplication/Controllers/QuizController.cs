using System.Text;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using QuizApplication.Handlers;
using QuizApplication.Models;

namespace QuizApplication.Controllers
{
    public class QuizController : Controller
    {
        private readonly IQuizHandler _quizHandler;

        public QuizController(IQuizHandler quizHandler)
        {
            _quizHandler = quizHandler;
        }
        
        public IActionResult Attempt()
        {
        
            // // Get the user's session
            // var session = HttpContext.Session;
            //
            // // Check if the session contains quiz attempt data
            // if (session.TryGetValue("quiz-attempt", out byte[] quizAttemptBytes))
            // {
            //     // The user has already started the quiz, retrieve the quiz attempt data
            //     var quizAttempt = JsonConvert.DeserializeObject<Quiz>(Encoding.UTF8.GetString(quizAttemptBytes));
            //
            //     // Display the quiz questions and answers
            //     // ...
            // }
            // else
            // {
            //     // The user has not started the quiz yet, create a new quiz attempt object
            //     var quizAttempt = new Quiz();
            //
            //     // Store the quiz attempt data in the session
            //     session.Set("quiz-attempt", Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(quizAttempt)));
            //
            //     // Display the quiz questions and answers
            //     // ...
            // }
            
            return View();
        }
        
        [HttpPost]
        public IActionResult Attempt(int questionNumber)
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