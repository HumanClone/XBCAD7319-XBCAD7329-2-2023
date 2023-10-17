using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Json;
using MVCAPP.Data;
using MVCAPP.Models;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace mvc_app.Controllers;

public class ReportingController : Controller{


    private static HttpClient sharedClient = new()
    {
        // TODO REPLACE WHEN DONE!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!1
        BaseAddress = new Uri("http://localhost:5173/api/"),
    };

    
    public IActionResult Index()
    {
        var statusCounts = sharedClient.GetFromJsonAsync<Dictionary<string, int>>("ticket/ticketStatusCount").Result;
        ReportingViewModel model = new ReportingViewModel();
        model.StatusCounts = statusCounts;
        return View(model);
    }

}