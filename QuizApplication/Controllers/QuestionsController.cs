using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizApplication.DbOperations;
using QuizApplication.Entities;
using QuizApplication.Models;
using QuizApplication.ViewModels;
using QuizApplication.ViewModels.QuestionViewModels;

namespace QuizApplication.Controllers
{
    [Authorize(Roles = "Admin, Manager")]
    public class QuestionsController : Controller
    {
        private readonly IQuestionRepository _questionRepository;

        public QuestionsController(IQuestionRepository questionRepository)
        {
            _questionRepository = questionRepository;
        }
        
        // List Questions
        public async Task<IActionResult> List()
        {
            var model = await _questionRepository.GetAllAsync();
            return View(model);
        }
        
        // Create Question
        public IActionResult CreateQuestion(int noOfAnswers)
        {
            return View(new CreateQuestionViewModel { AnswersCount = noOfAnswers });
        }

        [HttpPost]
        public async Task<IActionResult> CreateQuestion(CreateQuestionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var questionType = model.AnswersCount == 1 ? QuestionType.Text : QuestionType.Mcq;
            // loop through answers and add them to the question
            var question = new Question
            {
                Text = model.Question,
                QuestionType = questionType
            };
            
            if (model.AnswersCount != 1 && model.Answers.All(answer => !answer.IsCorrect))
            {
                ModelState.AddModelError("Answers", "Please select at least one correct answer.");
                return View(model);
            }

            question.AnswerOptions = model.Answers.Select(answer => new AnswerOption
            {
                Text = answer.Text,
                IsCorrect = answer.IsCorrect || model.AnswersCount == 1,
                AnswerNo = answer.AnswerNo
            }).ToList();

            await _questionRepository.AddAsync(question);
            
            if (!model.CreateAnother)
            {
                return RedirectToAction("List");
            }
            
            return RedirectToAction("CreateQuestion", routeValues:new {
                noOfAnswers = model.AnswersCount
            });
        }


        // Update Question
        public IActionResult EditQuestion()
        {
            return View();
        }


        // Delete Question
        public async Task<IActionResult> DeleteQuestion(int id)
        {
            var question = await _questionRepository.GetByIdAsync(id);
            if (question is null) return NotFound();

            await _questionRepository.DeleteAsync(question);
            return RedirectToAction("List");
        }
    }
}