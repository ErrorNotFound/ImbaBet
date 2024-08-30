namespace ImbaBetWeb.Models
{
    public struct MatchResult
    {
        public bool IsDraw {  get; set; }
        public Team? Winner { get; set; }
    }
}
