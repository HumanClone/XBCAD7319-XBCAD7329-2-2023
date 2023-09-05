using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MVCAPP.Data;
using MVCAPP.Models;


namespace mvc_app.Controllers;

public class StaffController:ControllerBase
{

    private readonly ILogger<StaffController> _logger;
    private static HttpClient sharedClient = new()
    {
        BaseAddress = new Uri("https://supportsystemapi.azurewebsites.net/api/"),
    };


    public StaffController(ILogger<StaffController> logger)
    {
        _logger = logger;
    }

    // public IActionResult Index()
    // {
    //     //return View();
    // }

    // public IActionResult Privacy()
    // {
    //     return View();
    // }
    

//returns a list of all tickets 
    [HttpGet]
    public async Task<IActionResult> allTickets()
    {
        try
        {
            HttpResponseMessage response = await sharedClient.GetAsync("ticket/tickets");

            if (response.IsSuccessStatusCode)
            {
                var tickets = await response.Content.ReadFromJsonAsync<List<TicketDetail>>();
                Console.WriteLine($"All tickets gotten  {response.StatusCode}");
                return null;

                //return(tickets);
            }
            else
            {
                Console.WriteLine($"Request failed with status code: {response.StatusCode}");
                return null;
                //return View();
            }
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Request error: {ex.Message}");
            return null;
            //return View();
        }
        
    }           

// this will return a list of the tickets that the current dev has assigned to them 
[HttpGet]
    public async Task<IActionResult> mytickets()
    {
        try
        {
            HttpResponseMessage response = await sharedClient.GetAsync("ticket/devTickets/?DevID="+HttpContext.Session.GetString("DevId"));

            if (response.IsSuccessStatusCode)
            {
                var tickets = await response.Content.ReadFromJsonAsync<List<TicketDetail>>();
                Console.WriteLine($"My tickets gotten {response.StatusCode}");
                return null;
                //return(tickets);
            }
            else
            {
                Console.WriteLine($"Request failed with status code: {response.StatusCode}");
                return null;
                //return View();
            }
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Request error: {ex.Message}");
            return null;
            //return View();
        }
    }


    //option for a button when they want to close a ticket 
    [HttpPost]
    public async Task<IActionResult> CloseTicket(IFormCollection form)
    {
        try
        {
            //TODO: depending how we access the model change the way to get TicketID
            HttpResponseMessage response = await sharedClient.GetAsync("ticket/closeTicket/?ticketID="+form["TicketID"]);

            if (response.IsSuccessStatusCode)
            {
                var ticket = await response.Content.ReadFromJsonAsync<TicketResponse>();
                Console.WriteLine($"Ticket Closed {response.StatusCode}");
                return null;
                //return(tickets);
            }
            else
            {
                Console.WriteLine($"Request failed with status code: {response.StatusCode}");
                return null;
                //return View();
            }
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Request error: {ex.Message}");
            return null;
            //return View();
        }
    }

    //TODO: based on view either use iformcollection or bind
    [HttpPost]
    public async Task<IActionResult> EditTicket(TicketDetail ticketDetail)
    {
        try
        {
            HttpResponseMessage response = await sharedClient.PostAsJsonAsync("ticket/closeTicket/?ticketID="+ticketDetail.TicketId,ticketDetail);

            if (response.IsSuccessStatusCode)
            {
                var ticket = await response.Content.ReadFromJsonAsync<TicketResponse>();
                Console.WriteLine($"Ticket edited: {response.StatusCode}");
                return null;
                //return(tickets);
            }
            else
            {
                Console.WriteLine($"Request failed with status code: {response.StatusCode}");
                return null;
                //return View();
            }
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Request error: {ex.Message}");
            return null;
            //return View();
        }
        
    }


    //takes the object of depending on how the view is created ,closes a ticket
    [HttpPost]
    public async Task<IActionResult> CreateTicket(TicketDetail ticketDetail, string categoryName)
    {
        ticketDetail.DevId=HttpContext.Session.GetString("DevId");
        ticketDetail.CategoryName=categoryName;
        ticketDetail.DateIssued=DateTime.UtcNow;

        try
        {
            HttpResponseMessage response = await sharedClient.PostAsJsonAsync("ticket/create",ticketDetail);

            if (response.IsSuccessStatusCode)
            {
                var ticket = await response.Content.ReadFromJsonAsync<TicketResponse>();
                Console.WriteLine($"Ticket created {response.StatusCode}");
                return null;
                //return(tickets);
            }
            else
            {
                Console.WriteLine($"Request failed with status code: {response.StatusCode}");
                return null;
                //return View();
            }
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Request error: {ex.Message}");
            return null;
            //return View();
        }


    }

    //gets a ticket based on the id and will return it or the view depending on use case 
    [HttpPost]
    public async Task<IActionResult> getTicket(string ticketID)
    {
        try
        {
            HttpResponseMessage response = await sharedClient.GetAsync("ticket/ticket/?ticketID="+ticketID);

            if (response.IsSuccessStatusCode)
            {
                var ticket = await response.Content.ReadFromJsonAsync<TicketDetail>();
                Console.WriteLine($"Ticket created {response.StatusCode}");
                return null;
                //return ticket;
                //return(ticket);
            }
            else
            {
                Console.WriteLine($"Request failed with status code: {response.StatusCode}");
                return null;
                //return View();
            }
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Request error: {ex.Message}");
            return null;
            //return View();
        }

    }


    //this method will get all the responses of a dev to a paticular ticket
    [HttpGet]
    public async Task<IActionResult> getMyResponses(string ticketID)
    {
        try
        {
            HttpResponseMessage response = await sharedClient.GetAsync("response/devticket/?DevID="+HttpContext.Session.GetString("DevId")+"&ticketID="+ticketID);

            if (response.IsSuccessStatusCode)
            {
                var ticket = await response.Content.ReadFromJsonAsync<List<TicketResponse>>();
                Console.WriteLine($"Ticket created {response.StatusCode}");
                return null;
                //return ticket;
                //return(ticket);
            }
            else
            {
                Console.WriteLine($"Request failed with status code: {response.StatusCode}");
                return null;
                //return View();
            }
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Request error: {ex.Message}");
            return null;
            //return View();
        }

    }


    //to get all the responses of a ticket 

    [HttpGet]

    public async Task<IActionResult> getTicketResponses(string ticketID)
    {
        try
        {
            HttpResponseMessage response = await sharedClient.GetAsync("response/ticket/?ticketID="+ticketID);

            if (response.IsSuccessStatusCode)
            {
                var ticket = await response.Content.ReadFromJsonAsync<List<TicketResponse>>();
                Console.WriteLine($"Ticket created {response.StatusCode}");
                return null;
                //return ticket;
                //return(ticket);
            }
            else
            {
                Console.WriteLine($"Request failed with status code: {response.StatusCode}");
                return null;
                //return View();
            }
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Request error: {ex.Message}");
            return null;
            //return View();
        }

    }


//method to send a response
//TODO: should have a field for email if they want to send it to another email 

[HttpPost]


    public async Task<IActionResult> SendResponse(string ticketID,string message)
    {
        MailRequest mr=new MailRequest();
        mr.Subject="Re+"+ticketID;
        mr.Body=message;
        mr.DevId=HttpContext.Session.GetString("DevId");

        try
        {
            HttpResponseMessage response = await sharedClient.PostAsJsonAsync("response/sendAdmin",mr);

            if (response.IsSuccessStatusCode)
            {
                var ticket = await response.Content.ReadFromJsonAsync<TicketResponse>();
                Console.WriteLine($"Response sent {response.StatusCode}");
                return null;
                //return(tickets);
            }
            else
            {
                Console.WriteLine($"Request failed with status code: {response.StatusCode}");
                return null;
                //return View();
            }
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Request error: {ex.Message}");
            return null;
            //return View();
        }

    }


}