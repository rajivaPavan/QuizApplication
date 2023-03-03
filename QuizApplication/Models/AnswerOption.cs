using System;

namespace QuizApplication.Models
{
    public class AnswerOption
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public bool IsCorrect { get; set; }
        public int QuestionId { get; set; }
        public DateTime CreatedAt { get; set; }
        public Question Question { get; set; }
    }
}