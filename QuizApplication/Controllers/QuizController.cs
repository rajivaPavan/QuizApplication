﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QuizApplication.Handlers;
using QuizApplication.Models;
using QuizApplication.ViewModels;

namespace QuizApplication.Controllers
{
    [Authorize]
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
        
        [HttpGet]
        public async Task<IActionResult> Attempt()
        {
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
        public async Task<IActionResult> Attempt(int questionNumber, string questionAnswer, int? isSubmit)
        {
            // get quiz id from session
            var quizId = GetQuizIdFromSession(HttpContext.Session);

            if (quizId != null)
            {
                var quiz = await _quizHandler.GetQuiz((int) quizId);
                
                // add and update the answer to the question
                quiz.QuizQuestions.First(q => q.QuestionNo == questionNumber).Answer = questionAnswer;
                quiz.AttemptedQuestionCount++;
                
                // update the question status
                await _quizHandler.UpdateQuiz(quiz);

                // is last question
                if (questionNumber == quiz.QuizQuestions.Count || isSubmit == 1)
                {
                    // set the finish time
                    quiz.FinishedAt = DateTime.Now;
                    
                    //remove quiz id from session
                    RemoveQuizIdInSession();
                    // calculate the quiz results
                    await _quizHandler.CalculateResults(quiz);
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

        public async Task<IActionResult> Result()
        {
            // get last attempted quiz of the user
            var quiz = await _quizHandler.GetLastQuizForUser(_userManager.GetUserId(User));
            return View(new QuizResultViewModel(quiz));
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