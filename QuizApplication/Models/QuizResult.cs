using System;

namespace QuizApplication.Models
{
    public class QuizResult
    {
        public int Id { get; set; }
        public int Score { get; set; }
        public DateTime TakenAt { get; set; }
    
        // Navigation properties
        public int QuizId { get; set; }
        public Quiz Quiz { get; set; }
    
        public string UserId { get; set; } // Added property
        public AppUser User { get; set; } // Navigation property to App
    }
}