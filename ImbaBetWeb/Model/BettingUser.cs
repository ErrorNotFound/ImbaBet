namespace ImbaBetWeb.Model
{
    public record BettingUser : IIdentifiable<int>
    {
        public int Id { get; set; }

        public Community? Community { get; set; } = null!;
        public int? MemberOfCommunityId { get; set; }

        public int Points { get; set; }
    }
}
