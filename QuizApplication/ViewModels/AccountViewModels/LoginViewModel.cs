﻿using System.ComponentModel.DataAnnotations;

namespace QuizApplication.ViewModels.AccountViewModels
{
    public class LoginViewModel
    {
        [Required]
        [DataType(DataType.Text)]
        public string Username { get; set; }
        
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}