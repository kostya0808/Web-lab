using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace WebLab.ViewComponents
{
    public class SystemStatsViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var model = new SystemStatsViewModel
            {
                ServerTime = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"),
                OsVersion = Environment.OSVersion.ToString(),
                ProcessMemoryMb = (Process.GetCurrentProcess().WorkingSet64 / (1024 * 1024)).ToString() + " MB",
                IsOnline = true
            };
            return View(model);
        }
    }

    public class SystemStatsViewModel
    {
        public string ServerTime { get; set; } = string.Empty;
        public string OsVersion { get; set; } = string.Empty;
        public string ProcessMemoryMb { get; set; } = string.Empty;
        public bool IsOnline { get; set; }
    }
}
