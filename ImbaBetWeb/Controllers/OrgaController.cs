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
    public class OrgaController(
        UserManager<MyIdentityUser> userManager,
        CommunityManager communityManager,
        PlayerManager playerManager) : ImbaBetControllerBase(userManager, playerManager)
    {
        private readonly CommunityManager _communityManager = communityManager;

        public async Task<IActionResult> MyCommunity()
        {
            var userResolve = await TryResolveUserAsync();
            if(!userResolve.Success)
            {
                return RedirectToAction("Error", "Home");
            }

            var communities = await _communityManager.GetCommunitiesAsync();
            var communityOfPlayer = await _communityManager.GetCommunityByIdAsync(userResolve.Player!.MemberOfCommunityId ?? -1);

            var vm = new MyCommunityViewModel()
            {
                Communities = [.. communities],
                Player = userResolve.Player!,
                CommunityOfPlayer = communityOfPlayer
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCommunity(string communityName)
        {
            var userResolve = await TryResolveUserAsync();
            if (!userResolve.Success)
            {
                return RedirectToAction("Error", "Home");
            }

            var communities = await _communityManager.GetCommunitiesAsync();

            var validator = new CommunityNameValidator(communities.Select(x => x.Name));
            var validationResult = validator.Validate(communityName);
            if(validationResult.IsValid)
            {
                await _communityManager.CreateCommunityAsync(userResolve.Player!, communityName);
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
            var userResolve = await TryResolveUserAsync();
            if (!userResolve.Success)
            {
                return RedirectToAction("Error", "Home");
            }

            var dropdownValue = Request.Form[OrgaConsts.Dropdown_CommunitySelection];
            if (string.IsNullOrEmpty(dropdownValue))
            {
                return RedirectToAction("Error", "Home");
            }
            var communityId = int.Parse(dropdownValue!);

            var wasSuccessful = await _communityManager.JoinCommunityAsync(userResolve.Player!, communityId);
            if(wasSuccessful)
            {
                this.SetSuccessAlert($"Community joined successfully.");
            }

            return RedirectToAction(nameof(MyCommunity));
        }

        [HttpGet]
        public async Task<IActionResult> LeaveCommunity()
        {
            var userResolve = await TryResolveUserAsync();
            if (!userResolve.Success)
            {
                return RedirectToAction("Error", "Home");
            }

            var community = await _communityManager.GetCommunityByIdAsync(userResolve.Player!.MemberOfCommunityId ?? -1);
            if (community != null)
            {
                var isOwner = community.OwnerId == userResolve.Player!.Id;
                if (isOwner)
                {
                    await _communityManager.DeleteCommunityOfPlayerAsync(userResolve.Player!);
                }
                else
                {
                    await _communityManager.LeaveCommunityAsync(userResolve.Player!);
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
        public async Task<IActionResult> KickMember(int playerId)
        {
            var userResolve = await TryResolveUserAsync();
            if (!userResolve.Success)
            {
                this.SetErrorAlert("Error while kicking user");
                return RedirectToAction(nameof(MyCommunity));
            }

            var userToBeKicked = await _playerManager.GetPlayerAsync(playerId);
            var community = await _communityManager.GetCommunityByIdAsync(userResolve.Player!.MemberOfCommunityId ?? -1);

            if (community == null || userToBeKicked == null)
            {
                this.SetErrorAlert("Error while kicking user");
                return RedirectToAction(nameof(MyCommunity));
            }
            //todo: make sure he is owner
            await _communityManager.KickMemberAsync(community.Id, userToBeKicked.Id);

            this.SetSuccessAlert($"{userToBeKicked.Id} has been kicked from Community.");

            return RedirectToAction(nameof(MyCommunity));
        }

        [HttpGet]
        [Route("Orga/PromoteToOwner/{userId}")]
        public async Task<IActionResult> PromoteToOwner(int playerId)
        {
            var userResolve = await TryResolveUserAsync();
            if (!userResolve.Success)
            {
                this.SetErrorAlert("Error while kicking user");
                return RedirectToAction(nameof(MyCommunity));
            }

            var idNewOwner = await _playerManager.GetPlayerAsync(playerId);
            var community = await _communityManager.GetCommunityByIdAsync(userResolve.Player!.MemberOfCommunityId ?? -1);

            if (community == null || idNewOwner == null)
            {
                return RedirectToAction(nameof(MyCommunity));
            }
            //todo: make sure he is owner
            await _communityManager.PromoteToOwnerAsync(community.Id, idNewOwner.Id);

            this.SetSuccessAlert($"{idNewOwner.Id} has been promoted to owner.");

            return RedirectToAction(nameof(MyCommunity));
        }

    }
}
