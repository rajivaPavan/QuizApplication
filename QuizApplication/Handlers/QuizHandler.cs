using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using QuizApplication.DbOperations;
using QuizApplication.Entities;
using QuizApplication.Models;

namespace QuizApplication.Handlers
{
    public class QuizHandler : IQuizHandler
    {
        private readonly IQuizRepository _quizRepository;
        private readonly IQuestionRepository _questionRepository;
        private readonly QuizSettings settings;

        public QuizHandler(IQuizRepository quizRepository, IQuestionRepository questionRepository, IOptionsSnapshot<QuizSettings> settings)
        {
            _quizRepository = quizRepository;
            _questionRepository = questionRepository;
            this.settings = settings.Value;
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
                QuizQuestions = await GetRandomQuestions(settings.QuestionCount),
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
            // list of unique random numbers
            var uniqueRandoms = new List<int>();
            var rand = new Random();
            while (uniqueRandoms.Count < count) {
                var num = rand.Next(0, questions.Count);
                if (!uniqueRandoms.Contains(num)) {
                    uniqueRandoms.Add(num);
                }
            }
            for (var i = 0; i < count; i++)
            {
                // get a random question which is not already added to the quiz
                var randomQuestion = questions[uniqueRandoms[i]];
               
                randomQuestions.Add(new QuizQuestion()
                {
                    Question = randomQuestion,
                    QuestionNo = i + 1,
                    StartedAt = null,
                    SubmittedAt = null,
                    CreatedAt = DateTime.Now,
                    
                });
            }

            return randomQuestions;
        }

        private async Task CalculateResults(Quiz quiz)
        {
            // check if quiz question is text or multiple choice
            // if text, check if answer is correct
            // if multiple choice, check if selected option is correct
            
            quiz.Score = 0;
            quiz.CorrectAnswerCount = 0;
            quiz.AttemptedQuestionCount = 0;
            
            for(int i=0; i<quiz.QuizQuestions.Count; i++)
            {
                var quizQuestion = quiz.QuizQuestions[i];
                var question = quizQuestion.Question;
                
                if (string.IsNullOrEmpty(quizQuestion.Answer)) continue;
                
                quiz.AttemptedQuestionCount += 1;
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
                    quiz = ScoreAnswer(quiz, quizQuestion);
                    quiz.CorrectAnswerCount += 1;
                }
                
                quiz.QuizQuestions[i] = quizQuestion;
            }
            
            // update quiz
            await _quizRepository.UpdateAsync(quiz);
        }

        /// <summary>
        /// Scores the answer and updates the quiz score
        /// </summary>
        /// <param name="quiz">The quiz to score</param>
        /// <param name="quizQuestion">The answer and the question</param>
        /// <returns>Quiz quiz</returns>
        private static Quiz ScoreAnswer(Quiz quiz, QuizQuestion quizQuestion)
        {
            var questionScore = 50.0;
            if (quizQuestion.SubmittedAt != null && quizQuestion.StartedAt != null)
            {
                var timeTaken = (quizQuestion.SubmittedAt - quizQuestion.StartedAt).Value.TotalSeconds;
                if (timeTaken < 60)
                {
                    questionScore += 50 * ((60 - timeTaken )/ 60);
                }
            }

            quiz.Score += questionScore;
            
            return quiz;
        }

        public async Task UpdateQuiz(Quiz quiz)
        {
            await _quizRepository.UpdateAsync(quiz);
        }

        public async Task<Quiz> GetLastQuizForUser(string userId)
        {
            var quizzes= await _quizRepository.GetAllAsync(q => q.UserId == userId);
            return quizzes.Any() ? quizzes.Last() : null;
        }

        public async Task SubmitAnswerToQuestion(Quiz quiz, int questionNumber, string questionAnswer)
        {
            // add and update the answer to the question
            var answeredQuestionIndex = quiz.QuizQuestions.FindIndex(q => q.QuestionNo == questionNumber);
            var answeredQuestion = quiz.QuizQuestions[answeredQuestionIndex];
            answeredQuestion.Answer = questionAnswer;
            answeredQuestion.SubmittedAt = DateTime.Now;
            quiz.AttemptedQuestionCount++;
            quiz.QuizQuestions[answeredQuestionIndex] = answeredQuestion;
            await UpdateQuiz(quiz);
        }

        public async Task FinishQuiz(Quiz quiz)
        {
            // set the finish time
            quiz.FinishedAt = DateTime.Now;
                    
            // calculate the quiz results
            await CalculateResults(quiz);
        }

        public async Task<List<Quiz>> GetLeaderBoard()
        {
            return await _quizRepository.GetFirstNQuizzes(10);
        }

        public async Task<int> GetUserRank(Quiz quiz)
        {
            return await _quizRepository.GetQuizRank(quiz);
        }

        public Task UpdateQuizQuestion(QuizQuestion quizQuestion)
        {
            return _quizRepository.UpdateQuizQuestion(quizQuestion);
        }
    }
}