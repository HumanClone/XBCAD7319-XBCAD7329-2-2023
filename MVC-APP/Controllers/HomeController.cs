using System.Diagnostics;
using BCrypt.Net;
using Microsoft.AspNetCore.Mvc;
using mvc_app.Models;
using MVCAPP.Models;

namespace mvc_app.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private static HttpClient sharedClient = new()
    {
        BaseAddress = new Uri("https://supportsystemapi.azurewebsites.net/api/"),
    };

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }
    public IActionResult Index()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

}
