namespace ImbaBetWeb.ViewModels.Admin
{
    public class ImportMatchplanViewModel
    {
        public required string MatchPlanInput { get; set; }

        public required Dictionary<string, string> Templates { get; set; }

        public required List<string>? ValidationErrors { get; set; }
    }
}
