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

public class AdminController : Controller
{
   // private static HttpClient sharedClient = new()
    // {
    //     BaseAddress = new Uri("https://supportsystemapi.azurewebsites.net/api/"),
    // };

    private static HttpClient sharedClient = new()
    {
        BaseAddress = new Uri("http://localhost:5173/api/"),
    };

    
    //Gets all tickerts from API for the admin
      [HttpGet]
        public async Task<IActionResult> ViewAdminTicket()
        {  
            string jsonTicketContent;
            List<TicketDetail> ticketList = new List<TicketDetail>();
           
            HttpResponseMessage response = await sharedClient.GetAsync("Ticket/getalltickets");
            if(response.IsSuccessStatusCode)
            {
                jsonTicketContent= await response.Content.ReadAsStringAsync();
                ticketList = JsonConvert.DeserializeObject<List<TicketDetail>>(jsonTicketContent);
                Console.WriteLine("Pull success");
            }
            else{
                Console.WriteLine("pull failed");
            }
            return View(ticketList);
        }
}