using System;
using System.Collections.Generic;

namespace QuizApplication.Models
{
    public class Quiz
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? FinishedAt { get; set; }
        public AppUser User { get; set; }
        public List<QuizQuestion> QuizQuestions { get; set; }
        
        
        public int? CorrectAnswerCount { get; set; }
        public int? AttemptedQuestionCount { get; set; }
        public int? Score { get; set; }

    }
}