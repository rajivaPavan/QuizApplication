using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.FeatureManagement;
using QuizApplication.Entities;
using QuizApplication.Handlers;
using QuizApplication.Models;
using QuizApplication.ViewModels.QuizViewModels;

namespace QuizApplication.Controllers
{
    [Authorize]
    public class QuizController : Controller
    {
        private readonly IQuizHandler _quizHandler;
        private readonly UserManager<AppUser> _userManager;
        private readonly IFeatureManager _featureManager;
        private const string SessionQuizIdKey = "quiz_id";
        private readonly QuizSettings _quizSettings;

        public QuizController(IQuizHandler quizHandler, UserManager<AppUser> userManager, 
            IFeatureManager featureManager, IOptions<QuizSettings> options)
        {
            _quizHandler = quizHandler;
            _userManager = userManager;
            _featureManager = featureManager;
            _quizSettings = options.Value;
        }
        
        [HttpGet]
        public async Task<IActionResult> Attempt()
        {
            if(!await QuizAccessAllowed(HttpContext))
                return RedirectToAction("Home");
            
            var quizId = GetQuizIdFromSession(HttpContext.Session);

            if (quizId != null)
            {
                var quiz = await _quizHandler.GetQuiz((int) quizId);
                // current quiz question
                var quizQuestion = quiz.QuizQuestions.First(q => !q.IsSubmitted());
                var model = new QuizViewModel(quiz, quizQuestion);

                return View(model);
            }

            // retrieve quiz from db
            return RedirectToAction("Start");
        }
        
        [HttpPost]
        public async Task<IActionResult> Attempt(int questionNumber, string questionAnswer, 
            int? isSubmit)
        {
            if(!await QuizAccessAllowed(HttpContext))
                return RedirectToAction("Home");
            
            // get quiz id from session
            var quizId = GetQuizIdFromSession(HttpContext.Session);

            if (quizId != null)
            {
                var quiz = await _quizHandler.GetQuiz((int) quizId);
                
                await _quizHandler.SubmitAnswerToQuestion(quiz, questionNumber, questionAnswer);
                

                // is last question
                if (questionNumber == quiz.QuizQuestions.Count || isSubmit == 1)
                {
                    await _quizHandler.FinishQuiz(quiz);

                    //remove quiz id from session
                    RemoveQuizIdInSession();
                    
                    // redirect to result page
                    return RedirectToAction("Result");
                }
                
                // increase the attempted question count
                var question = GetNextQuestion(quiz, questionNumber);

                var model = new QuizViewModel(quiz, question);
                
                return View(model);
            }
            
            return RedirectToAction("Start");
        }

        private static QuizQuestion GetNextQuestion(Quiz quiz, int questionNumber)
        {
            return quiz.QuizQuestions.First(q => q.QuestionNo == questionNumber + 1);
        }

        private void RemoveQuizIdInSession()
        {
            HttpContext.Session.Remove(SessionQuizIdKey);
        }

        [AllowAnonymous]
        public IActionResult Home()
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

        public async Task<IActionResult> Result()
        {
            // Check if the session contains quiz attempt data
            if (IsQuizInSession(HttpContext.Session))
            {
                return RedirectToAction("Attempt");
            }
            // get last attempted quiz of the user
            var quiz = await _quizHandler.GetLastQuizForUser(_userManager.GetUserId(User));
            if(quiz == null)
                return RedirectToAction("Home");
            return View(new QuizResultViewModel(quiz));
        }

        [AllowAnonymous]
        public IActionResult Leaderboard()
        {
            return View();
        }

        public async Task<IActionResult> Start()
        {
            if(!await QuizAccessAllowed(HttpContext))
                return RedirectToAction("Home");
            
            // check if user has already done the quiz
            var quiz = await _quizHandler.GetLastQuizForUser(_userManager.GetUserId(User));
            if (quiz != null)
                return RedirectToAction("Leaderboard");
            
            //Create a new quiz with random questions and saves it in database
            quiz = await _quizHandler.CreateQuizForUser(await _userManager.GetUserAsync(User));

            // store quiz id in session
            SetQuizIdInSession(HttpContext.Session, quiz.Id);

            return RedirectToAction("Attempt");
        }

        private async Task<bool> QuizAccessAllowed(HttpContext context)
        {
            var isAdmin = context.User.IsInRole(AppUserRoles.Admin);
            var quizAccess = await _featureManager.IsEnabledAsync(FeatureFlags.QuizAccess);
            // start 
            var start = DateTime.Parse(_quizSettings.QuizStartAt);
            // end
            var end = DateTime.Parse(_quizSettings.QuizEndAt);
            var now = DateTime.Now;
            var quizTime = start <= now && now <= end;
            return isAdmin || quizAccess && quizTime;
        }
    }

    
}