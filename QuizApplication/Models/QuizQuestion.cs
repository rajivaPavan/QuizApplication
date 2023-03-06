﻿using System;

namespace QuizApplication.Models
{
    public class QuizQuestion
    {
        public int Id { get; set; }
        public int QuizId { get; set; }
        public int QuestionId { get; set; }
        public string Answer { get; set; }
        public DateTime CreatedAt { get; set; }
        public Quiz Quiz { get; set; }
        public Question Question { get; set; }
    }
}