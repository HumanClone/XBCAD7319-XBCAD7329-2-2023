
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


namespace mvc_app.Controllers;

public class TicketController : Controller
{
    private static HttpClient sharedClient = new()
    {
         BaseAddress = new Uri("https://supportsystemapi.azurewebsites.net/api/"),
    };

    public TicketController()
    {
        
    }

    [HttpGet]
    public IActionResult Create(string categoryName)
    {
        ViewBag.SelectedCategoryName = categoryName;
        return View();
    }  

    [HttpPost]
    public async Task<IActionResult> Create(TicketDetail ticketDetail, string categoryName) 
    {
        //finds the selected category's id, call the api to get the category id
        var categoryId = await sharedClient.GetFromJsonAsync<int>($"category/getcategoryid?categoryName={categoryName}");     
        var userId = HttpContext.Session.GetInt32("UserId");
        ticketDetail.UserId = userId;

        try
        {
            if (ModelState.IsValid)
            {
                var Ticket = new TicketDetail()
                {
                    
                    TicketId = ticketDetail.TicketId,
                    CategoryId = categoryId.ToString(),
                    UserId = ticketDetail.UserId,
                    DevId = ticketDetail.DevId,
                    DateIssued = DateTime.UtcNow,
                    MessageContent = ticketDetail.MessageContent,
                    Status = ticketDetail.Status,
                    CategoryName = categoryName,
                    
                };//send this object to api
            

                try
                {
                    HttpResponseMessage response = await sharedClient.PostAsJsonAsync("ticket/createuserticket",Ticket);

                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("POST request successful");
                        Console.WriteLine("Added to database");
                        return RedirectToAction("ViewTicket");
                    }
                    else
                    {
                        Console.WriteLine($"Request failed with status code: {response.StatusCode}");
                        return View();
                    }

                }
                catch (HttpRequestException ex)
                {
                    Console.WriteLine($"Request error: {ex.Message}");
                    return View();
                }  
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
        [HttpGet]
        public async Task<IActionResult> ViewTicket()
        {
            var statuses = await PopulateStatusList();
            ViewData["StatusList"] = statuses;

            var tickets = new List<TicketDetail>();

            var role = HttpContext.Session.GetString("Role");
            if(role == "Student")
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                //call api to get student tickets

                tickets = sharedClient.GetFromJsonAsync<List<TicketDetail>>($"ticket/userTickets?userId={userId}").Result;            

                
            }
            else if(role == "Staff")
            {
                var devId = HttpContext.Session.GetInt32("DevId");
                //call api to get dev tickets

                tickets = sharedClient.GetFromJsonAsync<List<TicketDetail>>($"ticket/devtickets?devId={devId}").Result;            

            
            }else{
                //call api to get all tickets
                 tickets = sharedClient.GetFromJsonAsync<List<TicketDetail>>("ticket/getalltickets").Result;                     
            }

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

        private async Task<List<string>> PopulateStatusList()
        {
            var statusList = await sharedClient.GetFromJsonAsync<List<string>>("ticket/ticketstatuses");
            statusList.Insert(0, "All");
            return statusList;
        }

        public async Task<IActionResult> Filter(string startDate, string endDate, string status)
        {
            var statuses = await PopulateStatusList();
            ViewData["StatusList"] = statuses;
            
            //check if all the fields have been left as default
            if (startDate == null && endDate == null && status == "All")
            {
                return RedirectToAction("ViewTicket", "Ticket");
            }


            //get the role
            var role = HttpContext.Session.GetString("Role");

            //if the user is an employee
            if (role == "Staff")
            {
               var devIdString = HttpContext.Session.GetInt32("DevId").ToString();
               var filteredTickets = sharedClient.GetFromJsonAsync<List<TicketDetail>>($"ticket/filter?startDate={startDate}&endDate={endDate}&status={status}&userId={devIdString}&userRole={role}").Result; 
               return View("ViewTicket",filteredTickets);
            }
            else if (role == "Student")
            {
                var studentIdString = HttpContext.Session.GetInt32("UserId").ToString();
                var filteredTickets = sharedClient.GetFromJsonAsync<List<TicketDetail>>($"ticket/filter?startDate={startDate}&endDate={endDate}&status={status}&userId={studentIdString}&userRole={role}").Result;               
                //redirect to view ticket action and pass the filtered tickets
                return View("ViewTicket",filteredTickets);
            }
          
            return RedirectToAction("ViewTicket", "Ticket");

        }
        

        

}