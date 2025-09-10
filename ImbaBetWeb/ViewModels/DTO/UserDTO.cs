namespace ImbaBetWeb.ViewModels.DTO
{
    public class UserDTO
    {
        public required string Username { get; set; }

        public required string Id { get; set; }

        public required string Email { get; set; }

        public bool EmailConfirmed { get; set; }

        public int? MemberOfCommunityId { get; set; }

        public bool IsAdmin { get; set; }
        public bool IsEditor { get; set; }
    }
}
