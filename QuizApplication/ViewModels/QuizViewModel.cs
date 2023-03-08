using Microsoft.AspNetCore.Mvc;
using QuizApplication.Models;

namespace QuizApplication.ViewModels
{
    public class QuizViewModel
    {
        public int QuizId { get; set; }
        public QuizQuestion QuizQuestion { get; set; }
        
        [FromForm]
        public int QuizAnswer { get; set; }
        
        [FromForm]
        public int QuestionNumber { get; set; }
        
        [FromQuery]
        public bool isSubmit { get; set; }
    }
    
    
}