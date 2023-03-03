using QuizApplication.DbContext;
using QuizApplication.Models;

namespace QuizApplication.DbOperations
{
    public interface IAnswerOptionRepository : IRepository<AnswerOption>
    {
        
    }
    
    public class AnswerOptionRepository : Repository<AnswerOption>, IAnswerOptionRepository
    {
        public AnswerOptionRepository(AppDbContext context) : base(context)
        {
        }
    }
}