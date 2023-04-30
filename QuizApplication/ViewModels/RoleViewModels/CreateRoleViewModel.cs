using System.ComponentModel.DataAnnotations;

namespace QuizApplication.ViewModels.RoleViewModels
{
    public class CreateRoleViewModel
    {
        [Required] public string RoleName { get; set; }
    }
}