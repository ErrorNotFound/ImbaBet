namespace ImbaBetWeb.ViewModels.Admin
{
    public class ImportMatchplanViewModel
    {
        public string MatchPlanInput { get; set; }

        public Dictionary<string, string> Templates { get; set; }

        public List<string>? ValidationErrors { get; set; }
    }
}
