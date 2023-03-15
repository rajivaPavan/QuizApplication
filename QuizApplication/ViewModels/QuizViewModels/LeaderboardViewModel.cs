using System.Collections.Generic;
using QuizApplication.Models;

namespace QuizApplication.ViewModels.QuizViewModels
{
    public class LeaderboardViewModel
    {
        public LeaderboardViewModel(List<Quiz> quizzes)
        {
            Leaderboard = new List<QuizResultViewModel>();
            foreach (var quiz in quizzes)
            {
                Leaderboard.Add(new QuizResultViewModel(quiz));
            }
        }

        public QuizResultViewModel QuizResult { get; set; }
        public List<QuizResultViewModel> Leaderboard { get; set; }
        public int UserRank { get; set; }
    }
}