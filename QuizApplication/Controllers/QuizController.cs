using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using QuizApplication.Handlers;
using QuizApplication.Models;
using QuizApplication.ViewModels;

namespace QuizApplication.Controllers
{
    public class QuizController : Controller
    {
        private readonly IQuizHandler _quizHandler;
        private readonly UserManager<AppUser> _userManager;
        private const string SessionQuizIdKey = "quiz_id";

        public QuizController(IQuizHandler quizHandler, UserManager<AppUser> userManager)
        {
            _quizHandler = quizHandler;
            _userManager = userManager;
        }
        
        public async Task<IActionResult> Attempt()
        {
            var quizId = GetQuizIdFromSession(HttpContext.Session);

            if (quizId != null)
            {
                var quiz = await _quizHandler.GetQuiz((int) quizId);
                // current quiz question
                var quizQuestion = quiz.QuizQuestions.First(q => !q.isSubmitted());
                var model = new QuizViewModel()
                {
                    QuizId = quiz.Id,
                    QuestionNumber = quizQuestion.QuestionNo,
                    QuizQuestion = quizQuestion
                };

                return View(model);
            }

            // retrieve quiz from db
            return RedirectToAction("Start");
        }
        
        [HttpPost]
        public async Task<IActionResult> Attempt(QuizViewModel model)
        {
            // get quiz id from session
            var quizId = GetQuizIdFromSession(HttpContext.Session);

            if (quizId != null)
            {
                var quiz = await _quizHandler.GetQuiz((int) quizId);
                
                // is last question
                if (model.QuestionNumber == quiz.QuizQuestions.Count)
                {
                    //remove quiz id from session
                    RemoveQuizIdInSession();
                    // calculate the quiz results
                    await _quizHandler.CalculateResults(quiz);
                    // redirect to result page
                    return RedirectToAction("Result");
                }
                
                // increase the attempted question count
                await _quizHandler.IncreaseAttemptedQCount(quiz);
                
                model = UpdateQuestionInView(model, quiz);

                return View(model);
            }
            
            return View(model);
        }

        private static QuizViewModel UpdateQuestionInView(QuizViewModel model, Quiz quiz)
        {
            // update view model with next question
            var quizQuestion = quiz.QuizQuestions.First(q => q.QuestionNo == model.QuestionNumber + 1);

            model.QuizQuestion = quizQuestion;

            return model;
        }

        private void RemoveQuizIdInSession()
        {
            HttpContext.Session.Remove(SessionQuizIdKey);
        }

        public IActionResult Instructions()
        {
            // Check if the session contains quiz attempt data
            if (IsQuizInSession(HttpContext.Session))
            {
                return RedirectToAction("Attempt");
            }

            return View();
        }

        private static bool IsQuizInSession(ISession session)
        {
            return GetQuizIdFromSession(session) != null;
        }

        private static int? GetQuizIdFromSession(ISession session)
        {
            return session.GetInt32(SessionQuizIdKey);
        }

        private static void SetQuizIdInSession(ISession session, int quizId)
        {
            session.SetInt32(SessionQuizIdKey, quizId);
        }

        public IActionResult Result()
        {
            return View();
        }
        
        public IActionResult Leaderboard()
        {
            return View();
        }

        public async Task<IActionResult> Start()
        {
            //Create a new quiz with random questions and saves it in database
            var quiz = await _quizHandler.CreateQuizForUser(await _userManager.GetUserAsync(User));

            // store quiz id in session
            SetQuizIdInSession(HttpContext.Session, quiz.Id);

            return RedirectToAction("Attempt");
        }
    }

    
}