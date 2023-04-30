using System;
using System.Collections.Generic;
using QuizApplication.Entities;

namespace QuizApplication.Models
{
    public class Question
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string ImageUrl { get; set; }
        public List<AnswerOption> AnswerOptions { get; set; }
        public QuestionType QuestionType { get; set; }
    }
}