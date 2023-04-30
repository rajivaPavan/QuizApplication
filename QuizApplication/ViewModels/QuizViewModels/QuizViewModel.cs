using QuizApplication.Models;
using QuizApplication.ViewModels.QuestionViewModels;

namespace QuizApplication.ViewModels.QuizViewModels
{
    public class QuizViewModel
    {
        public QuizViewModel()
        {
            
        }

        public QuizViewModel(Quiz quiz, QuizQuestion quizQuestion)
        {
            QuizId = quiz.Id;
            QuizQuestion = quizQuestion;
            QuestionImage = QuestionViewModel.DecorateUrl(quizQuestion.Question.ImageUrl);
            QuestionNumber = quizQuestion.QuestionNo;
            QuestionCount = quiz.QuizQuestions.Count;
            AnswerCount = quizQuestion.Question.AnswerOptions.Count;
        }

        public string QuestionImage { get; set; }

        public int QuizId { get; set; }
        public QuizQuestion QuizQuestion { get; set; }

        public int QuestionNumber { get; set; }
        public int AnswerCount { get; set; }
        public int QuestionCount { get; set; }
    }
    
    
}