using QuizApplication.DbOperations;

namespace QuizApplication.Handlers
{
    public interface IQuizHandler
    {
        
    }
    
    public class QuizHandler : IQuizHandler
    {
        private readonly IQuizRepository _quizRepository;
        private readonly IQuestionRepository _questionRepository;

        public QuizHandler(IQuizRepository quizRepository, IQuestionRepository questionRepository)
        {
            _quizRepository = quizRepository;
            _questionRepository = questionRepository;
        }
        
        
    }
}