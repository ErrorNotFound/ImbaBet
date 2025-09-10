using ImbaBetWeb.Business;
using ImbaBetWeb.Business.Extensions;
using ImbaBetWeb.Model;
using ImbaBetWeb.Validation;
using ImbaBetWeb.ViewModels.Orga;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ImbaBetWeb.Controllers
{
    [Authorize]
    public class OrgaController : Controller
    {
        private readonly UserManager<MyIdentityUser> _userManager;
        private readonly CommunityManager _communityManager;
        private readonly DatabaseManager _databaseManager;
        private readonly PlayerManager _playerManager;

        public OrgaController( 
            UserManager<MyIdentityUser> userManager,
            CommunityManager communityManager,
            DatabaseManager databaseManager,
            PlayerManager playerManager)
        {
            _userManager = userManager;
            _communityManager = communityManager;
            _databaseManager = databaseManager;
            this._playerManager = playerManager;
        }


        public async Task<IActionResult> MyCommunity()
        {
            var communities = await _communityManager.GetCommunitiesAsync();
            var idUser = await _userManager.GetUserAsync(User);
            if(idUser == null)
            {
                return RedirectToAction("Error", "Home");
            }

            var player = await _playerManager.GetPlayerAsync(idUser.PlayerId);
            if (player == null)
            {
                return RedirectToAction("Error", "Home");
            }

            var vm = new MyCommunityViewModel()
            {
                Communities = communities,
                Player = player
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCommunity(string communityName)
        {
            var idUser = await _userManager.GetUserAsync(User);
            if (idUser == null)
            {
                return RedirectToAction("Error", "Home");
            }

            var player = await _playerManager.GetPlayerAsync(idUser.PlayerId);
            if (player == null)
            {
                return RedirectToAction("Error", "Home");
            }

            var communities = await _communityManager.GetCommunitiesAsync();

            var validator = new CommunityNameValidator(communities.Select(x => x.Name));
            var validationResult = validator.Validate(communityName);
            if(validationResult.IsValid)
            {
                await _communityManager.CreateCommunityAsync(player, communityName);
            }
            else
            {
                foreach (var error in validationResult.Errors)
                {
                    this.SetErrorAlert(error.ErrorMessage);
                }
            }

            return RedirectToAction(nameof(MyCommunity));
        }

        [HttpPost]
        public async Task<IActionResult> JoinCommunity()
        {
            var idUser = await _userManager.GetUserAsync(User);
            if (idUser == null)
            {
                return RedirectToAction("Error", "Home");
            }

            var player = await _playerManager.GetPlayerAsync(idUser.PlayerId);
            if (player == null)
            {
                return RedirectToAction("Error", "Home");
            }

            var dropdownValue = Request.Form[OrgaConsts.Dropdown_CommunitySelection];
            if (string.IsNullOrEmpty(dropdownValue))
            {
                return RedirectToAction("Error", "Home");
            }
            var communityId = int.Parse(dropdownValue!);

            var wasSuccessful = await _communityManager.JoinCommunityAsync(player, communityId);
            if(wasSuccessful)
            {
                this.SetSuccessAlert($"Community joined successfully.");
            }

            return RedirectToAction(nameof(MyCommunity));
        }

        [HttpGet]
        public async Task<IActionResult> LeaveCommunity()
        {
            var idUser = await _userManager.GetUserAsync(User);
            if (idUser == null)
            {
                return RedirectToAction("Error", "Home");
            }

            var player = await _playerManager.GetPlayerAsync(idUser.PlayerId);
            if (player == null)
            {
                return RedirectToAction("Error", "Home");
            }

            var community = player.Community;
            if (community != null)
            {
                var isOwner = community.OwnerId == player.Id;
                if (isOwner)
                {
                    await _communityManager.DeleteCommunityOfPlayerAsync(player);
                }
                else
                {
                    await _communityManager.LeaveCommunityAsync(player);
                }
            }
            // todo
            /*
            var wasSuccessful = isOwner ? await _communityManager.DeleteCommunityOfUserAsync(user) : await _communityManager.LeaveCommunityAsync(user);
            if (wasSuccessful)
            {
                this.SetSuccessAlert($"Community ({community!.Name}) has been left successfully.");
            }*/

            return RedirectToAction(nameof(MyCommunity));
        }

        [HttpGet]
        [Route("Orga/KickMember/{userId}")]
        public async Task<IActionResult> KickMember(string userId)
        {
            var idUser = await _userManager.GetUserAsync(User);
            if (idUser == null)
            {
                return RedirectToAction("Error", "Home");
            }
            var player = await _playerManager.GetPlayerAsync(idUser.PlayerId);
            if (player == null)
            {
                return RedirectToAction("Error", "Home");
            }
            var userToBeKicked = await _userManager.FindByIdAsync(userId);
            var community = player.Community;

            if(community == null || idUser == null || userToBeKicked == null)
            {
                this.SetErrorAlert("Error while kicking user");
                return RedirectToAction(nameof(MyCommunity));
            }
            //todo: make sure he is owner
            await _communityManager.KickMemberAsync(community.Id, userToBeKicked.PlayerId);

            this.SetSuccessAlert($"{userToBeKicked.UserName} has been kicked from Community.");

            return RedirectToAction(nameof(MyCommunity));
        }

        [HttpGet]
        [Route("Orga/PromoteToOwner/{userId}")]
        public async Task<IActionResult> PromoteToOwner(string userId)
        {
            var idUser = await _userManager.GetUserAsync(User);
            if (idUser == null)
            {
                return RedirectToAction("Error", "Home");
            }
            var player = await _playerManager.GetPlayerAsync(idUser.PlayerId);
            if (player == null)
            {
                return RedirectToAction("Error", "Home");
            }
            var idNewOwner = await _userManager.FindByIdAsync(userId);
            var community = player?.Community;

            if (community == null || idNewOwner == null)
            {
                return RedirectToAction(nameof(MyCommunity));
            }
            //todo: make sure he is owner
            await _communityManager.PromoteToOwnerAsync(player!.Community!.Id, idNewOwner.PlayerId);

            this.SetSuccessAlert($"{idNewOwner.UserName} has been promoted to owner.");

            return RedirectToAction(nameof(MyCommunity));
        }

    }
}
