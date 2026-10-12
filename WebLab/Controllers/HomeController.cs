using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using WebLab.Models;
using WebLab.Services;

namespace WebLab.Controllers;

public class HomeController : Controller
{
    private readonly IEmailSender _emailSender;
    private readonly ILogger<HomeController> _logger;
    private readonly IWebHostEnvironment _env;

    public HomeController(IEmailSender emailSender, ILogger<HomeController> logger, IWebHostEnvironment env)
    {
        _emailSender = emailSender;
        _logger = logger;
        _env = env;
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
        ViewBag.CurrentCulture = CultureInfo.CurrentUICulture.NativeName;

        _logger.LogInformation("Loaded Home page for user: {User}, Culture: {Culture}", sessionUser, CultureInfo.CurrentUICulture.Name);

        return View();
    }

    [HttpPost]
    public IActionResult SetLanguage(string culture, string returnUrl)
    {
        Response.Cookies.Append(
            CookieRequestCultureProvider.DefaultCookieName,
            CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
            new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
        );
        _logger.LogInformation("Language changed to: {Culture}", culture);
        return LocalRedirect(returnUrl);
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
        ViewBag.CurrentCulture = CultureInfo.CurrentUICulture.NativeName;
        return View("Index");
    }

    [HttpGet]
    public IActionResult Upload()
    {
        var filesPath = Path.Combine(_env.WebRootPath, "Files");
        var fileNames = Directory.Exists(filesPath)
            ? Directory.GetFiles(filesPath).Select(Path.GetFileName).Where(f => f != ".gitkeep").ToList()
            : new List<string?>();
        ViewBag.Files = fileNames;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> UploadFile(IFormFile? file)
    {
        if (file != null && file.Length > 0)
        {
            var uploadsFolder = Path.Combine(_env.WebRootPath, "Files");
            Directory.CreateDirectory(uploadsFolder);
            var filePath = Path.Combine(uploadsFolder, file.FileName);
            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);
            ViewBag.UploadSuccess = "Файл успішно завантажено: " + file.FileName;
            _logger.LogInformation("File uploaded successfully: {FileName}", file.FileName);
        }
        else
        {
            ViewBag.UploadError = "Будь ласка, виберіть файл для завантаження.";
        }

        var filesPath = Path.Combine(_env.WebRootPath, "Files");
        var fileNames = Directory.Exists(filesPath)
            ? Directory.GetFiles(filesPath).Select(Path.GetFileName).Where(f => f != ".gitkeep").ToList()
            : new List<string?>();
        ViewBag.Files = fileNames;
        return View("Upload");
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
