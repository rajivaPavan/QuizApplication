using System.Collections.Generic;
using System.Threading.Tasks;
using QuizApplication.Models;

namespace QuizApplication.Handlers
{
    public interface IQuizHandler
    {
        /// <summary>
        /// Get the quiz with all questions and answers
        /// </summary>
        /// <param name="quizId"></param>
        /// <returns></returns>
        Task<Quiz> GetQuiz(int quizId);
        
        /// <summary>
        /// Create a quiz with random questions for the specified user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<Quiz> CreateQuizForUser(AppUser user);
        
        /// <summary>
        /// Get the last quiz for the specified user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<Quiz> GetLastQuizForUser(string userId);
        
        /// <summary>
        /// Submit the answer to the specified question
        /// </summary>
        /// <param name="quiz"></param>
        /// <param name="questionNumber"></param>
        /// <param name="questionAnswer"></param>
        /// <returns></returns>
        Task SubmitAnswerToQuestion(Quiz quiz, int questionNumber, string questionAnswer);
        
        /// <summary>
        /// Set finish time and calculate results and save to db
        /// </summary>
        /// <param name="quiz"></param>
        /// <returns></returns>
        Task FinishQuiz(Quiz quiz);

        Task<List<Quiz>> GetLeaderBoard();
        Task<int> GetUserRank(Quiz quiz);
    }
}