using ImbaBetWeb.Model;
using ImbaBetWeb.ViewModels.DTO;

namespace ImbaBetWeb.ViewModels.Admin
{
    public class AccountsViewModel
    {
        public required List<UserDTO> Users { get; set; }

        public required List<Community> Communities { get; set; }
    }
}
