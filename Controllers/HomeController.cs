using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TesteRedisStateSession.Models;

namespace TesteRedisStateSession.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
        private readonly IDistributedCache _cache;

        public HomeController(ILogger<HomeController> logger, IDistributedCache cache)
		{
			_logger = logger;
            _cache = cache;
        }

		public async Task<IActionResult> Index()
		{
			await HttpContext.Session.LoadAsync();

            HttpContext.Session.SetString("MinhaVarDeSessao", "Minha Variavel de Sessão");

			var sessionstartTime = HttpContext.Session.GetString("storedSessionStartTime");

			if (sessionstartTime == null)
			{
				sessionstartTime = DateTime.Now.ToString("HH:mm:ss:ffff");
				HttpContext.Session.SetString("storedSessionStartTime", sessionstartTime);
				await HttpContext.Session.CommitAsync();
			}

			ViewBag.SessionStartTime = sessionstartTime;
			ViewBag.CachedTimeUTC = Encoding.UTF8.GetString( _cache.Get("cachedTimeUTC"));

			return View();
		}

		public IActionResult Privacy()
		{
			ViewBag.CachedTimeUTC = Encoding.UTF8.GetString(_cache.Get("cachedTimeUTC"));

			var minhaVarDeSessao = HttpContext.Session.GetString("MinhaVarDeSessao");

			if (minhaVarDeSessao != null)
			{
                ViewBag.MinhaVarDeSessao = minhaVarDeSessao;

			}

			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
