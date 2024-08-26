using ImbaBetWeb.Models;
using ImbaBetWeb.Models.Consts;
using ImbaBetWeb.ViewModels.DTO;
using Microsoft.AspNetCore.Identity;

namespace ImbaBetWeb.ViewModels.Admin
{
    public class AccountsViewModel
    {
        public IList<UserDTO> Users { get; set; }

        public IList<Community> Communities { get; set; }
    }

    public class UserDTO
    {
        public string Username { get; set; }

        public string Id { get; set; }

        public string Email { get; set; }

        public int? MemberOfCommunityId { get; set; }

        public bool IsAdmin { get; set; }
        public bool IsEditor { get; set; }
    }
}
