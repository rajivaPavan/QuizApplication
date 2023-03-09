using System.Collections;
using Microsoft.AspNetCore.Mvc;
using QuizApplication.Models;

namespace QuizApplication.ViewModels
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
            QuestionNumber = quizQuestion.QuestionNo;
            QuestionCount = quiz.QuizQuestions.Count;
            AnswerCount = quizQuestion.Question.AnswerOptions.Count;
        }
        
        public int QuizId { get; set; }
        public QuizQuestion QuizQuestion { get; set; }

        public int QuestionNumber { get; set; }
        public int AnswerCount { get; set; }
        public int QuestionCount { get; set; }
    }
    
    
}