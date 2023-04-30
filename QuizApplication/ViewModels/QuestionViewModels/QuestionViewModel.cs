using QuizApplication.Models;

namespace QuizApplication.ViewModels.QuestionViewModels
{
    public abstract class QuestionViewModel
    {
        public static string DecorateUrl(string url)
        {
            //TODO: Change this to a more robust solution
            return url != null ? $"https://questions-gsbmcdd5ezdcb0bd.z01.azurefd.net/moramaths/{url}" : null;
        }
    }
}