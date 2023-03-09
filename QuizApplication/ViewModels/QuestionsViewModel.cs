using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using QuizApplication.Entities;
using QuizApplication.Models;

namespace QuizApplication.ViewModels
{
    public class QuestionsViewModel
    {
        public int NoOfAnswerOptions { get; set; }
        public QuestionType Type { get; set; }
        public List<Question> Questions { get; set; }
    }
}