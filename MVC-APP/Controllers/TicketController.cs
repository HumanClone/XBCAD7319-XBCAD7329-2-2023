
using System.Net;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MVCAPP.Data;
using MVCAPP.Models;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Newtonsoft;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc.ViewFeatures;


namespace mvc_app.Controllers;

public class TicketController : Controller
{
    private readonly ApplicationDbContext _context;

    public TicketController(ApplicationDbContext context)
    {
       this._context = context;
    }

      private static HttpClient sharedClient = new()
    {
        BaseAddress = new Uri("https://supportsystemapi.azurewebsites.net/api/"),
    };

    [HttpGet]
        public IActionResult Create(string categoryName)
        {
           
            ViewBag.SelectedCategoryName = categoryName;
            return View();
        }  
    
     [HttpPost]
     public async Task<IActionResult> Create(TicketDetail ticketDetail, string categoryName) 
        {
            var httpClient = new HttpClient();
            try
            {
             //finds the selected category's id 
               int categoryId = _context.Categories
               .Where(c => c.CategoryName == categoryName)
               .Select(c=> c.CategoryId)
               .FirstOrDefault();

                if (ModelState.IsValid)
                {
                    var Ticket = new TicketDetail()
                    {
                        
                        TicketId = ticketDetail.TicketId,
                        CategoryId = categoryId.ToString(),
                        UserId = ticketDetail.UserId,
                        DevId = ticketDetail.DevId,
                        DateIssued = DateTime.Now,
                        MessageContent = ticketDetail.MessageContent,
                        Status = ticketDetail.Status,
                        CategoryName = categoryName,
                       
                        
                    };//send this object to api

            /*HttpResponseMessage response = httpClient.PostAsJsonAsync(
                            "API/Ticket/createUserTicket", Ticket);
                            response.EnsureSuccessStatusCode();

                    HttpContent content = new StringContent(Ticket.ToString(), Encoding.UTF8,"application/json");

                    var response = httpClient.PostAsJsonAsync("API/Ticket/createTicket", Ticket).Result;*/
                            
                    //_context.TicketDetails.Add(Ticket);
                    //_context.SaveChanges();

                    // Serialize the Ticket object to JSON
                    var serializedTicket = JsonSerializer.Serialize(Ticket);

                    Console.WriteLine(serializedTicket);

                    // API endpoint URL
                    

                    
                        // Create the HTTP content with the serialized JSON
                        var content = new StringContent(serializedTicket, Encoding.UTF8, "application/json");

                    try{
                        var response = await sharedClient.PostAsync("Ticket/createuserTicket", content);

                        // Handle the response as needed
                        if (response.IsSuccessStatusCode)
                        {
                            Console.WriteLine("Ticket sent successfully.");
                            return RedirectToAction("ViewTicket");
                        }
                        else
                        {
                            Console.WriteLine("Failed to send ticket. Status code: " + response.StatusCode);
                            return View();
                        }
                    }
                    catch(Exception ex)
                    {
                        TempData["errorMessage"] = ex.Message;
                        return View();
                    }
                        // Send the POST request
                    Console.WriteLine("Added to database");
                }
                else
                {
                    foreach (var modelState in ModelState.Values)
                        {
                            foreach (var error in modelState.Errors)
                            {
                                Console.WriteLine($"Model Error: {error.ErrorMessage}");
                            }
                        }

                    return View();
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();

            }
        }


    // check when able to, not sure if correct but cannot test yet
    /*static async Task<Uri> CreateTicketDetailAsync(TicketDetail ticketDetail)
    {
        HttpResponseMessage response = await client.PostAsJsonAsync(
            "API/Ticket/createTicket", ticketDetail);
        response.EnsureSuccessStatusCode();

        // return URI of the created resource.
        return response.Headers.Location;
    }
*/
        [HttpGet]
        public IActionResult ViewTicket()
        {
            var tickets = _context.TicketDetails.ToList();
            List<TicketDetail> ticketList = new List<TicketDetail>();

            foreach (var ticket in tickets)
            {
                var ticketDetail = new TicketDetail
                {
                    TicketId = ticket.TicketId,
                    CategoryName = ticket.CategoryName,
                    MessageContent = ticket.MessageContent,
                    DateIssued = ticket.DateIssued,
                    Status = ticket.Status
                };
              
                ticketList.Add(ticketDetail);
            }

            return View(ticketList);
        }
}