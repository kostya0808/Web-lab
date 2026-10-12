using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebLab.Models;
using WebLab.Services;

namespace WebLab.Controllers;

public class HomeController : Controller
{
    private readonly IEmailSender _emailSender;

    public HomeController(IEmailSender emailSender)
    {
        _emailSender = emailSender;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> SendEmail(EmailViewModel model)
    {
        if (ModelState.IsValid)
        {
            await _emailSender.SendEmailAsync(model.Email, "Нове повідомлення з сайту", model.Message);
            ViewBag.Success = "Email успішно надіслано!";
        }
        return View("Index");
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
