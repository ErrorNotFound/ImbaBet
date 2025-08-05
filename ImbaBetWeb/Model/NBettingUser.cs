using ImbaBetWeb.Models;

namespace ImbaBetWeb.Model
{
    public record NBettingUser
    {
        public int Id { get; set; }
        public int? MemberOfCommunityId { get; set; }

        public int Points { get; set; }

        public int RemainingRenames { get; set; }

        public string? ProfilePicturePath { get; set; }
    }
}
