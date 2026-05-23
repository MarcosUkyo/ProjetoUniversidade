using Microsoft.AspNetCore.Mvc;
using ProjetoUniversidade.Autenticacao;
using ProjetoUniversidade.Models;
using System.Diagnostics;

namespace ProjetoUniversidade.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            if (!HttpContext.Session.GetInt32(SessionKeys.UserId).HasValue)
                return RedirectToAction("Login", "Auth");
            return View();
        }

        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() =>
            View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
    }
}
