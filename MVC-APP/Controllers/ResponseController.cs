using System.Net;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Json;
using MVCAPP.Data;
using MVCAPP.Models;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;


using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;


namespace mvc_app.Controllers;

public class ResponseController : Controller
{
    private static HttpClient sharedClient = new()
    {
        BaseAddress = new Uri("https://supportsystemapi.azurewebsites.net/api/"),
    };

    [HttpPost]
    public async Task<IActionResult> SendDevResponse(string ticketID, string message, string email)
    {
        MailRequest mr = new MailRequest();

        mr.Subject = "Re+" + ticketID;
        mr.Body = message;
        mr.DevId = HttpContext.Session.GetString("DevId");
        mr.ToEmail = email;

        try
        {
            HttpResponseMessage response = await sharedClient.PostAsJsonAsync("response/sendadmin", mr);

            if(response.IsSuccessStatusCode)
            {
                var ticket = await response.Content.ReadFromJsonAsync<TicketResponse>();
                Console.WriteLine($"Response sent {response.StatusCode}");
                return null;
            }
            else
            {
                Console.WriteLine($"Request failed with status code: {response.StatusCode}");
                return null;
            }
        }
        catch (System.Exception)
        {
            
            throw;
        }
    }

    [HttpPost]
    public async Task<IActionResult> SendUserResponse(string ticketID, string message)
    {
        MailRequest mr = new MailRequest();

        mr.Subject = "Re+" + ticketID;
        mr.Body = message;
        mr.UserId = HttpContext.Session.GetString("UserId");

        try
        {
            HttpResponseMessage response = await sharedClient.PostAsJsonAsync("response/senduser", mr);

            if(response.IsSuccessStatusCode)
            {
                var ticket = await response.Content.ReadFromJsonAsync<TicketResponse>();
                Console.WriteLine($"Response sent {response.StatusCode}");
                return null;
            }
            else
            {
                Console.WriteLine($"Request failed with status code: {response.StatusCode}");
                return null;
            }
        }
        catch (System.Exception)
        {
            
            throw;
        }
    }
}