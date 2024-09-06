using ImbaBetWeb.Logic.Extensions;
using ImbaBetWeb.Validation;
using ImbaBetWeb.ViewModels;
using ImbaBetWeb.ViewModels.Home;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ImbaBetWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger, IEmailSender emailSender, IConfiguration configuration)
        {
            _logger = logger;
            _emailSender = emailSender;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Rules()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View(new ContactViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Contact(ContactViewModel vm)
        {
            var validator = new ContactViewModelValidator();
            var validationResult = validator.Validate(vm);
            
            if (validationResult.IsValid)
            {
                var subject = "[ImbaBet] New contact message";
                var content = $"Name: {vm.Name} <br>" +
                    $"Email: {vm.EMail} <br>" +
                    $"Subject: {vm.Subject} <br>" +
                    $"Message:<br>{vm.Message!.Replace("\r\n", "<br>")}";

                var targetEMail = _configuration.GetSection("Configuration")["ContactEMail"];
                if (string.IsNullOrEmpty(targetEMail)) 
                {
                    this.SetErrorAlert("Contact form is not configured.");
                    return View(vm);
                }

                try
                {
                    await _emailSender.SendEmailAsync(targetEMail, subject, content);
                    if (vm.SendCopy)
                    {
                        await _emailSender.SendEmailAsync(vm.EMail!, subject, content);
                    }
                    this.SetSuccessAlert("E-Mail has been sent successfully.");
                }
                catch (Exception)
                {
                    this.SetErrorAlert("Error while sending your email.");
                    return View(vm);
                }                

                return RedirectToAction(nameof(Contact));
            }
            else
            {
                foreach (var error in validationResult.Errors)
                {
                    this.SetErrorAlert(error.ErrorMessage);
                }
                return View(vm);
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
