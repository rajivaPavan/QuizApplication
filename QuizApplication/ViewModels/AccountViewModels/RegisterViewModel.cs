using System.ComponentModel.DataAnnotations;

namespace QuizApplication.ViewModels.AccountViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [DataType(DataType.Text)]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d{6})\S+$",
         ErrorMessage = "Given Index Number has an Invalid format")]
        public string Username { get; set; }
        
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        
        [Required]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }

        [Required]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string Name { get; set; }
    }
}