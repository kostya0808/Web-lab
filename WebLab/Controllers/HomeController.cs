using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebLab.Models;
using WebLab.Services;

namespace WebLab.Controllers;

public class HomeController : Controller
{
    private readonly IEmailSender _emailSender;
    private readonly ILogger<HomeController> _logger;

    public HomeController(IEmailSender emailSender, ILogger<HomeController> logger)
    {
        _emailSender = emailSender;
        _logger = logger;
    }

    public IActionResult Index(string? user, string? info)
    {
        if (!string.IsNullOrEmpty(user))
        {
            HttpContext.Session.SetString("UserName", user);
            _logger.LogInformation("Query parameter user received: {User}", user);
        }

        if (!Request.Cookies.ContainsKey("LastVisit"))
        {
            Response.Cookies.Append("LastVisit", DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"));
        }

        string sessionUser = HttpContext.Session.GetString("UserName") ?? "Гість";
        string lastVisit = Request.Cookies["LastVisit"] ?? DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");

        ViewBag.SessionUser = sessionUser;
        ViewBag.LastVisit = lastVisit;
        ViewBag.QueryInfo = info;

        _logger.LogInformation("Loaded Home page for user: {User}", sessionUser);

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> SendEmail(EmailViewModel model)
    {
        if (ModelState.IsValid)
        {
            await _emailSender.SendEmailAsync(model.Email, "Нове повідомлення з сайту", model.Message);
            ViewBag.Success = "Email успішно надіслано!";
            _logger.LogInformation("Email sent to {Email}", model.Email);
        }
        ViewBag.SessionUser = HttpContext.Session.GetString("UserName") ?? "Гість";
        ViewBag.LastVisit = Request.Cookies["LastVisit"] ?? DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
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
