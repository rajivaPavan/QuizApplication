using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace QuizApplication.ViewModels.QuestionViewModels
{
    public class CreateQuestionViewModel
    {
        [Required]
        public string Question { get; set; }

        [FromForm]
        // list of answers
        public List<AnswerOptionDto> Answers { get; set; }

        // create another
        [Display(Name = "Create Another")]
        public bool CreateAnother { get; set; }

        public int AnswersCount { get; set; }
        
        [Display(Name = "Image File Name")]
        public string ImageFileName { get; set; }
    }
    
    public class AnswerOptionDto
    {
        [Required]
        public string Text { get; set; }

        [Display(Name = "Answer")]
        public bool IsCorrect { get; set; }
        public int AnswerNo { get; set; }
    }
}