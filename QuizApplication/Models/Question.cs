using System;
using System.Collections.Generic;

namespace QuizApplication.Models
{
    public class Question
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public ICollection<AnswerOption> AnswerOptions { get; set; }
    }
}