namespace ImbaBetWeb.Model
{
    public record Player : IIdentifiable<int>
    {
        public int Id { get; set; }

        public int? MemberOfCommunityId { get; set; }

        public int Points { get; set; }

        public string? ProfilePicturePath { get; set; }
    }
}
