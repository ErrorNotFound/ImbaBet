namespace ImbaBetWeb.Model.Questions
{
    public record Question : IIdentifiable<int>
    {
        public int Id { get; set; }

        public string Text { get; set; } = string.Empty;
 
        public QuestionType Type { get; set; }

        public DateTime DueDate { get; set; }

        public bool IsOver { get; set; }

        public string? ChoicesRaw { get; set; }

        public IEnumerable<string> Choices
        {
            get
            {
                if (string.IsNullOrWhiteSpace(ChoicesRaw))
                {
                    return Enumerable.Empty<string>();
                }
                return ChoicesRaw.Split(';').Select(choice => choice.Trim()).Where(choice => !string.IsNullOrWhiteSpace(choice));
            }
        }

        public string? CorrectAnswer { get; set; }
        public int Points { get; set; }
    }
}
