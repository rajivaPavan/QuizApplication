using System;
using System.Collections.Generic;
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
            IFeatureManager featureManager, IOptionsSnapshot<QuizSettings> options)
        {
            _quizHandler = quizHandler;
            _userManager = userManager;
            _featureManager = featureManager;
            _quizSettings = options.Value;
            _quizSettings.QuizEndAt ??= "2023/03/13 00:00:00";
            _quizSettings.QuizStartAt ??= "2021/03/16 00:00:00";
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
                var quizQuestion = await GetNextQuestion(quiz, 0);
                if (quizQuestion.QuestionNo == 1 && quizQuestion.StartedAt == null)
                {
                    DateTimeOffset d = DateTimeOffset.UtcNow;
                    long time = d.ToUnixTimeMilliseconds();
                    Response.Cookies.Append("start", time.ToString());
                }

                // updated started at of quizQuestion
                //quizQuestion.StartedAt = DateTime.Now;
                // update quizQuestion in db
                //await _quizHandler.UpdateQuizQuestion(quizQuestion);
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
                var question = await GetNextQuestion(quiz, questionNumber);

                var model = new QuizViewModel(quiz, question);
                

                return View(model);
            }
            
            return RedirectToAction("Start");
        }

        /// <summary>
        /// Update the quiz question with the current time and return the next question
        /// </summary>
        /// <param name="quiz"></param>
        /// <param name="questionNumber"></param>
        /// <returns></returns>
        private async Task<QuizQuestion> GetNextQuestion(Quiz quiz, int questionNumber)
        {
            var nextq = quiz.QuizQuestions.First(q => q.QuestionNo == questionNumber + 1);
            nextq.StartedAt = DateTime.Now;
            await _quizHandler.UpdateQuizQuestion(nextq);
            return nextq;
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
        public async Task<IActionResult> Leaderboard()
        {
            var quizzes = await _quizHandler.GetLeaderBoard();
            var model = new LeaderboardViewModel(quizzes);

            if (User.Identity.IsAuthenticated)
            {
                var quiz = await _quizHandler.GetLastQuizForUser(_userManager.GetUserId(User));
                model.QuizResult = new QuizResultViewModel(quiz);
                model.UserRank = await _quizHandler.GetUserRank(quiz);
            }
            
            return View(model);
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