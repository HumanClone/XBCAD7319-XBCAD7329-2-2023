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

        //Get the 7 days for the current week
        var dates = getDatesForWeek();


        //Would need to call this in a loop
        //var datesAndCounts = sharedClient.GetFromJsonAsync<List<TicketStatus>>("ticket/countByStatusAndDate").Result;
        ReportingViewModel model = new ReportingViewModel();
        model.StatusCounts = statusCounts;
        return View(model);
    }


    private List<DateTime> getDatesForWeek()
    {
        DateTime today = DateTime.Today;
        int currentDayOfWeek = (int)today.DayOfWeek;
        int startOfWeek = (int)DayOfWeek.Monday;
        DateTime startDate = today.AddDays(-currentDayOfWeek + startOfWeek);
        DateTime endDate = startDate.AddDays(6);
        
        List<DateTime> weekDates = new List<DateTime>();
        for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
        {
            weekDates.Add(date);
        }

        return weekDates;

    }

}