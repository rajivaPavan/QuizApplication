using System.ComponentModel.DataAnnotations;

namespace QuizApplication.RequestDTOs
{
    public class LoginDTO
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }
}