using QuizApplication.DbContext;
using QuizApplication.Models;

namespace QuizApplication.DbOperations
{
    public interface IQuizResultRepository : IRepository<QuizResult>
    {
        
    }
    
    public class QuizResultRepository : Repository<QuizResult>, IQuizResultRepository
    {
        public QuizResultRepository(AppDbContext context) : base(context)
        {
            
        }
    }
}