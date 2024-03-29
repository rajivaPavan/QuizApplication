﻿using QuizApplication.Models;

namespace QuizApplication.ViewModels.QuizViewModels
{
    public class QuizResultViewModel
    {
        public QuizResultViewModel(Quiz quiz)
        {
            if (quiz.User != null) Name = quiz.User.Name;
            Score = quiz.Score ?? 0;
            StartedAt = quiz.CreatedAt.ToString("dd/MM/yyyy HH:mm");
            FinishedAt = quiz.FinishedAt?.ToString("dd/MM/yyyy HH:mm") ?? "Not finished";
            if (quiz.QuizQuestions != null) NumberOfQuestions = quiz.QuizQuestions.Count;
            NoOfQuestionsAttempted = quiz.AttemptedQuestionCount ?? 0;
            NoOfQuestionsCorrect = quiz.CorrectAnswerCount ?? 0;
        }

        public double Score { get; set; }
        public string StartedAt { get; set; }
        public string FinishedAt { get; set; }
        public int NumberOfQuestions { get; set; }
        public int NoOfQuestionsAttempted { get; set; }
        public int NoOfQuestionsCorrect { get; set; }
        public string Name { get; set; }
    }
}