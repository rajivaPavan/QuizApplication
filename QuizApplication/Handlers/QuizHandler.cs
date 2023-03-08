using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using QuizApplication.DbOperations;
using QuizApplication.Models;

namespace QuizApplication.Handlers
{
    public interface IQuizHandler
    {
        Task<Quiz> GetQuiz(int quizId);
        Task<Quiz> CreateQuizForUser(AppUser user);
        Task CalculateResults(Quiz quiz);
        Task IncreaseAttemptedQCount(Quiz quiz);
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


        public async Task<Quiz> GetQuiz(int quizId)
        {
            return await _quizRepository.GetAsync(e => e.Id == quizId);
        }

        public async Task<Quiz> CreateQuizForUser(AppUser user)
        {
            // create a quiz from 3 random questions selected from db
            var quiz = new Quiz()
            {
                QuizQuestions = await GetRandomQuestions(),
                CreatedAt = DateTime.Now,
                User = user,
                UserId = user.Id
            };

            quiz = await _quizRepository.AddAsync(quiz);
            
            // add random questions to quiz
            // quiz.QuizQuestions = await GetRandomQuestions(quiz);
            // quiz = await _quizRepository.UpdateAsync(quiz);
            
            return quiz;
        }

        private async Task<List<QuizQuestion>> GetRandomQuestions(int count = 3)
        {
            // get random questions from db
            var questions = await _questionRepository.GetAllAsync();
            var randomQuestions = new List<QuizQuestion>();
            var random = new System.Random();
            for (int i = 0; i < count; i++)
            {
                var randomQuestion = questions[random.Next(0, questions.Count)];
                randomQuestions.Add(new QuizQuestion()
                {
                    Question = randomQuestion,
                    QuestionNo = i + 1,
                    StartedAt = null,
                    SubmittedAt = null,
                    CreatedAt = DateTime.Now
                });
            }

            return randomQuestions;
        }

        public Task CalculateResults(Quiz quiz)
        {
            throw new System.NotImplementedException();
        }

        public Task IncreaseAttemptedQCount(Quiz quiz)
        {
            throw new System.NotImplementedException();
        }
    }
}