using ImbaBetWeb.Business;
using ImbaBetWeb.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ImbaBetWeb.Controllers
{
    public class ImbaBetControllerBase : Controller
    {
        protected readonly UserManager<MyIdentityUser> _userManager;
        protected readonly PlayerManager _playerManager;

        public ImbaBetControllerBase(
            UserManager<MyIdentityUser> userManager,
            PlayerManager playerManager)
        {
            _userManager = userManager;
            _playerManager = playerManager;
        }

        protected async Task<(bool Success, MyIdentityUser? User, Player? Player)> TryResolveUserAsync()
        {
            var identityUser = await _userManager.GetUserAsync(User);
            if (identityUser == null)
            {
                return (false, null, null);
            }

            var player = await _playerManager.GetPlayerAsync(identityUser.PlayerId);
            if (player == null)
            {
                return (false, identityUser, null);
            }

            return (true, identityUser, player);
        }
    }
}
