using QuizApplication.DbContext;
using QuizApplication.Models;

namespace QuizApplication.DbOperations
{
    public interface IQuizQuestionRepository : IRepository<QuizQuestion>
    {
        
    }
    
    public class QuizQuestionRepository: Repository<QuizQuestion>, IQuizQuestionRepository
    {
        public QuizQuestionRepository(AppDbContext context) : base(context)
        {
        }
    }
}