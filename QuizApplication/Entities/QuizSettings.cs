namespace QuizApplication.Entities
{
    public class QuizSettings
    {
        public int QuestionCount { get; set; }
        public int TimeLimitInSeconds { get; set; }
        public string QuizStartAt { get; set; }
        public string QuizEndAt { get; set; }
    }
}