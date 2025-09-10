namespace ImbaBetWeb.Business.Ranking.Details
{
    public class CommunityDetails
    {
        public required string Name { get; set; }
        public int MemberCount { get; set; }
        public decimal AveragePoints { get; internal set; }
    }
}
