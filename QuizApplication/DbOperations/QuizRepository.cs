﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using QuizApplication.DbContext;
using QuizApplication.Models;

namespace QuizApplication.DbOperations
{
    public interface IQuizRepository : IRepository<Quiz>
    {
        Task<Quiz> GetFullQuiz(int quizId);
        Task<List<Quiz>> GetFirstNQuizzes(int n);
        Task<int> GetQuizRank(Quiz quiz);
        Task UpdateQuizQuestion(QuizQuestion quizQuestion);
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

        public Task<List<Quiz>> GetFirstNQuizzes(int n)
        {
            // get first n quizzes from db descending order of score
            return _context.Quizzes.Include(q => q.User)
                .OrderByDescending(q => q.Score).Take(n).ToListAsync();
        }

        public async Task<int> GetQuizRank(Quiz quiz)
        {
            // after ordering by score, get the rank of the user
            var q = await _context.Quizzes.OrderByDescending(q => q.Score).ToListAsync();
            int count = q.Count();
            for (int i = 0; i< count; i++)
            {
                if(q[i].Id == quiz.Id)
                {
                    return i+1;
                }
            }
            return count;
        }

        public Task UpdateQuizQuestion(QuizQuestion quizQuestion)
        {
            _context.QuizQuestions.Update(quizQuestion);
            return _context.SaveChangesAsync();
        }
    }
}