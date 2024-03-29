﻿using System;

namespace QuizApplication.Models
{
    public class QuizQuestion
    {
        /// <summary>
        /// Primary key
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Foreign key
        /// </summary>
        public int QuizId { get; set; }
        
        /// <summary>
        /// Foreign key
        /// </summary>
        public int QuestionId { get; set; }
        public int QuestionNo { get; set; }
        public string Answer { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? SubmittedAt { get; set; }
        public Quiz Quiz { get; set; }
        public Question Question { get; set; }
        public bool? IsCorrect { get; set; }

        public bool IsSubmitted()
        {
            return SubmittedAt != null;
        }
    }
}