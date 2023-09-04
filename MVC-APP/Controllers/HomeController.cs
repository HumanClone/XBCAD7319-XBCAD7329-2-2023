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

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }


        /// How to Secure Passwords with BCrypt.NET
        /// [
        ///  var passwordHash = BCrypt.HashPassword("Password123!");
        /// ]
        /// https://code-maze.com/dotnet-secure-passwords-bcrypt/
        /// Acccessed[1 September 2023]


    [HttpPost]
    public async Task<IActionResult> Login(IFormCollection form)
    {
        //TODO: Change form names to match form
        UserLogin cred= new UserLogin();
        cred.Email = form["Email"];
        var passhash=BCrypt.Net.BCrypt.HashPassword(form["UserPassword"]);
        cred.Password = passhash;

        try
        {
            HttpResponseMessage response = await sharedClient.PostAsJsonAsync("users/Login",cred);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("POST request successful");
                try
                {
                    var user = await response.Content.ReadFromJsonAsync<UserInfo>();
                    HttpContext.Session.SetInt32("UserId", user.UserId);
                    HttpContext.Session.SetString("Name", user.Name);
                    HttpContext.Session.SetString("Email", user.Email);

                    //TODO: Redirect to user screen 
                    return View();
                    
                }
                //this will catch if they return a dev team object instead 
                catch(Exception ex)
                {
                    var user = await response.Content.ReadFromJsonAsync<TeamDev>();
                    HttpContext.Session.SetInt32("DevId", user.DevId);
                    HttpContext.Session.SetString("Name", user.Name+" "+user.Surname);
                    HttpContext.Session.SetString("Email", user.Email);
                    //TODO: Redirect to dev screeen 
                    return View();
                }
            }
            else
            { 
                Console.WriteLine($"Request failed with status code: {response.StatusCode}");
                ViewBag.Notification = "Password or Username Incorrect";
                return View();
            
            }
        }
        catch (HttpRequestException ex)
        {
            ViewBag.Notification = "Error ocureed try again";
            Console.WriteLine($"Request error: {ex.Message}");
            return View();
        }
    }
}
