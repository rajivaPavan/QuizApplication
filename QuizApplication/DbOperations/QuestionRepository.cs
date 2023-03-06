using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QuizApplication.DbContext;
using QuizApplication.Models;

namespace QuizApplication.DbOperations
{
    public interface IQuestionRepository : IRepository<Question>
    {
        
    }

    public class QuestionRepository : Repository<Question>, IQuestionRepository
    {
        private readonly AppDbContext _context;

        public QuestionRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public Task<List<Question>> GetAllAsync(IEnumerable<string> includes = null)
        {
            var query = _context.Questions.Include(q => q.AnswerOptions);
            return query.ToListAsync();
        }
    }
}