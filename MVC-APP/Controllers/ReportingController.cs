using System;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Json;
using MVCAPP.Data;
using MVCAPP.Models;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Newtonsoft.Json;

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
        
        var ticketStatuses = new List<TicketStatus>();

        //Get the 7 days for the current week
        var dates = getDatesForWeek();

        
        foreach(var status in statusCounts.Keys)
        {     
            var ticketPoints = new TicketStatus();
            ticketPoints.Name = status;
            var dateCounts = new List<TicketData>();
            for(int i = 0; i < dates.Count; i++){
                var date = dates[i].ToString("yyyy-MM-dd");
                var dateAndCount = sharedClient.GetFromJsonAsync<TicketData>($"ticket/countByStatusAndDate?status={status}&date={date}").Result;
                var ticketData = new TicketData
                {
                    Date = dateAndCount.Date,
                    Count = dateAndCount.Count
                };  
                         
                dateCounts.Add(ticketData);
            }
            
            ticketPoints.Tickets = dateCounts;

            ticketStatuses.Add(ticketPoints);
        }

        model.TicketStatuses = ticketStatuses;    
        return View(model);
    }


    private List<DateTime> getDatesForWeek()
    {
        // DateTime today = DateTime.Today;
        // int currentDayOfWeek = (int)today.DayOfWeek;
        // int startOfWeek = (int)DayOfWeek.Monday;
        // DateTime startDate = today.AddDays(-currentDayOfWeek + startOfWeek);
        // DateTime endDate = startDate.AddDays(6);
        
        // List<DateTime> weekDates = new List<DateTime>();
        // for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
        // {
        //     weekDates.Add(date);
        // }

        // TODO : Change to currentdates
        DateTime startDate = new DateTime(2023, 9, 5); // Set the start date to September 5th, 2023
        DateTime endDate = new DateTime(2023, 9, 11); // Set the end date to September 11th, 2023
        
        List<DateTime> weekDates = new List<DateTime>();
        for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
        {
            weekDates.Add(date);
        }

        return weekDates;

    }

}