using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QuizApplication.DbContext;
using QuizApplication.Models;

namespace QuizApplication.DbOperations
{
    public interface IQuizRepository : IRepository<Quiz>
    {
        Task<Quiz> GetFullQuiz(int quizId);
    }
    
    public class QuizRepository : Repository<Quiz>, IQuizRepository
    {
        private readonly AppDbContext _context;

        public QuizRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Quiz> GetFullQuiz(int quizId)
        {
           return await _context.Quizzes.Include(q => q.QuizQuestions)
                .ThenInclude(qq => qq.Question)
                .ThenInclude(q => q.AnswerOptions)
                .FirstOrDefaultAsync(q => q.Id == quizId);
        }
    }
}