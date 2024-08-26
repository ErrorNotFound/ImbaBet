// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using ImbaBetWeb.Data;
using ImbaBetWeb.Logic;
using ImbaBetWeb.Logic.Helper;
using ImbaBetWeb.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ImbaBetWeb.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly DatabaseManager _databaseManager;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public IndexModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            DatabaseManager databaseManager,
            IConfiguration configuration,
            IWebHostEnvironment webHostEnvironment)
        {
            
            _userManager = userManager;
            _signInManager = signInManager;
            _databaseManager = databaseManager;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Display(Name = "Username")]
            public string Username { get; set; }
        }

        [BindProperty]
        public FileUploadModel FileUpload { get; set; }

        public class FileUploadModel
        {
            [Required]
            [Display(Name = "File")]
            public IFormFile FormFile { get; set; }
        }

        private async Task LoadAsync(ApplicationUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);

            Input = new InputModel
            {
                Username = userName
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            
            // todo: fix.  ModelState should only validate the corresponding model
            /*
            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }*/
            
            if (Input.Username != user.UserName && user.RemainingRenames > 0)
            {
                var setUserNameResult = await _userManager.SetUserNameAsync(user, Input.Username);
                if (!setUserNameResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set username.";
                    return RedirectToPage();
                }
                user.RemainingRenames -= 1;
                await _userManager.UpdateAsync(user);
			}

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeletePictureAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await _databaseManager.DeleteProfilePicture(user.Id);

            StatusMessage = "Your profile picture has been deleted";

            return RedirectToPage("./Index");
        }

        public async Task<IActionResult> OnPostUploadAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            // todo: fix.  ModelState should only validate the corresponding model
            if (!ModelState.IsValid)
            {
                StatusMessage = "Please correct the form.";

                return Page();
            }

            var allowedExtensions = _configuration["ProfilePictureUpload:UploadAllowedExtensions"].Split(';');
            var fileSizeLimit = int.Parse(_configuration["ProfilePictureUpload:FileSizeLimitInBytes"]);

            var formFileContent = await FileHelpers.ProcessFormFile<FileUploadModel>(
                    FileUpload.FormFile, ModelState, allowedExtensions,
                    fileSizeLimit);

            if (!ModelState.IsValid)
            {
                StatusMessage = "Please correct the form.";

                return Page();
            }

            var trustedFileNameForFileStorage = Path.GetRandomFileName();
            var relativePath = _configuration["ProfilePictureUpload:UploadDirectory"] + trustedFileNameForFileStorage + Path.GetExtension(FileUpload.FormFile.FileName);
            var filePathAbsolute = _webHostEnvironment.WebRootPath + relativePath;

            using (var fileStream = System.IO.File.Create(filePathAbsolute))
            {
                await fileStream.WriteAsync(formFileContent);
                
            }

            await _databaseManager.DeleteProfilePicture(user.Id);

            user.ProfilePicturePath = relativePath;
            await _userManager.UpdateAsync(user);

            return RedirectToPage("./Index");
        }
    }

}

