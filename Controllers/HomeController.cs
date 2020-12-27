using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TesteRedisStateSession.Models;

namespace TesteRedisStateSession.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;

		public HomeController(ILogger<HomeController> logger)
		{
			_logger = logger;
		}

		public async Task<IActionResult> Index()
		{
			await HttpContext.Session.LoadAsync();

			var sessionstartTime = HttpContext.Session.GetString("storedSessionStartTime");

			if (sessionstartTime == null)
			{
				sessionstartTime = DateTime.Now.ToLongTimeString();
				HttpContext.Session.SetString("storedSessionStartTime", sessionstartTime);
				await HttpContext.Session.CommitAsync();
			}

			ViewBag.SessionStartTime = sessionstartTime;

			return View();
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
}
