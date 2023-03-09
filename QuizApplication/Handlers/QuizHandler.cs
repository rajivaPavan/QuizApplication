using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using QuizApplication.DbOperations;
using QuizApplication.Entities;
using QuizApplication.Models;

namespace QuizApplication.Handlers
{
    public interface IQuizHandler
    {
        Task<Quiz> GetQuiz(int quizId);
        Task<Quiz> CreateQuizForUser(AppUser user);
        Task CalculateResults(Quiz quiz);
        Task UpdateQuiz(Quiz quiz);
        Task<Quiz> GetLastQuizForUser(string userId);
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
            return await _quizRepository.GetFullQuiz(quizId);
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
            var random = new Random();
            for (var i = 0; i < count; i++)
            {
                // get a random question which is not already added to the quiz
                var randomQuestion = questions[random.Next(questions.Count)];
               
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

        public async Task CalculateResults(Quiz quiz)
        {
            // check if quiz question is text or multiple choice
            // if text, check if answer is correct
            // if multiple choice, check if selected option is correct
            
            quiz.Score = 0;
            
            for(int i=0; i<quiz.QuizQuestions.Count; i++)
            {
                var quizQuestion = quiz.QuizQuestions[i];
                var question = quizQuestion.Question;
                if (!string.IsNullOrEmpty(quizQuestion.Answer))
                {
                    quizQuestion.IsCorrect = false;
                    
                    if (question.QuestionType == QuestionType.Text )
                    {
                        if (question.AnswerOptions[0].Text == quizQuestion.Answer)
                        {
                            quizQuestion.IsCorrect = true;
                        }
                    }
                    else if (question.QuestionType == QuestionType.Mcq && !string.IsNullOrEmpty(quizQuestion.Answer))
                    {
                        // get the correct option
                        var correctOption = question.AnswerOptions.Find(o => o.IsCorrect);
                        if (correctOption != null && correctOption.AnswerNo.ToString() == quizQuestion.Answer)
                        {
                            quizQuestion.IsCorrect = true;
                        }
                    }
                    
                    if ((bool) quizQuestion.IsCorrect)
                    {
                        quiz.Score += 1;
                    }
                }
            }
            // update quiz
            await _quizRepository.UpdateAsync(quiz);
        }

        public async Task UpdateQuiz(Quiz quiz)
        {
            await _quizRepository.UpdateAsync(quiz);
        }

        public async Task<Quiz> GetLastQuizForUser(string userId)
        {
            return await _quizRepository.GetAsync(q => q.UserId == userId);
        }
    }
}