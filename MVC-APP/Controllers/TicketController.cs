
using System.Net;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Json;
using MVCAPP.Models;
using System.Text;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Net.Mime;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.IdentityModel.Tokens;


namespace mvc_app.Controllers;

public class TicketController : Controller
{
    private static HttpClient sharedClient = new()
    {
         BaseAddress = new Uri("https://supportsystemapi.azurewebsites.net/api/"),
    };

    // private static HttpClient sharedClient = new()
    // {
    //     // TODO REPLACE WHEN DONE!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!1
    //     BaseAddress = new Uri("http://localhost:5173/api/"),
    // };

    // private static HttpClient sharedClient = new()
    // {
    //     BaseAddress = new Uri("http://localhost:5173/api/"),
    // };

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
    public async Task<IActionResult> Create(TicketDetail ticketDetail, string categoryName,List<IFormFile> files) 
    {
        //finds the selected category's id, call the api to get the category id
        var categoryId = await sharedClient.GetFromJsonAsync<int>($"category/getcategoryid?categoryName={categoryName}");     
        var userId = HttpContext.Session.GetInt32("UserId");
        ticketDetail.UserId = userId;
        Console.WriteLine("Category:"+categoryId);
        try
        {
            if (ModelState.IsValid)
            {
                using MultipartFormDataContent multipartContent = new();
                if(HttpContext.Session.GetString("DevId")!=null)
                {
                    var devId = HttpContext.Session.GetInt32("DevId");
                    multipartContent.Add(new StringContent(devId+"",Encoding.UTF8, MediaTypeNames.Text.Plain),"DevId");
                }
                else{
                    multipartContent.Add(new StringContent(userId+"",Encoding.UTF8, MediaTypeNames.Text.Plain),"UserId");
                }
                var Ticket = new TicketDetail()
                {
                    
                    TicketId = ticketDetail.TicketId,
                    CategoryId = categoryId.ToString(),
                    DateIssued = DateTime.UtcNow,
                    MessageContent = ticketDetail.MessageContent,
                    Status = ticketDetail.Status,
                    CategoryName = categoryName,
                    
                };

                
                multipartContent.Add(new StringContent(Ticket.CategoryId,Encoding.UTF8, MediaTypeNames.Text.Plain),"CategoryId");
                multipartContent.Add(new StringContent(Ticket.UserId+"",Encoding.UTF8, MediaTypeNames.Text.Plain),"UserId");
                multipartContent.Add(new StringContent(Ticket.MessageContent,Encoding.UTF8, MediaTypeNames.Text.Plain),"MessageContent");
                multipartContent.Add(new StringContent(Ticket.CategoryName,Encoding.UTF8, MediaTypeNames.Text.Plain),"CategoryName");

                Console.WriteLine(files.Count.ToString());
               
                if(!files.IsNullOrEmpty())
                {
                    foreach(var file in files)
                    {
                        var streamContent = new StreamContent(file.OpenReadStream());
                        streamContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType.ToString());
                        streamContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                        {
                            Name = "Attachments",
                            FileName = file.FileName
                        };
                        multipartContent.Add(streamContent, "Attachments");

                    }

                    foreach(var item in multipartContent)
                    {
                        Console.WriteLine(item.Headers.ToString());
                    }
                }

                try
                {
                    var response = await sharedClient.PostAsync("ticket/createuserticket",multipartContent);

                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("POST request successful");
                        Console.WriteLine("Added to database");
                        return RedirectToAction("ViewTicket");
                    }
                    else
                    {
                        Console.WriteLine($"Request failed with status code: {response.RequestMessage.ToString()}");
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
            var userId = HttpContext.Session.GetInt32("UserId");
            var devId = HttpContext.Session.GetInt32("DevId");
            if (userId == null && devId == null)
            {
                return RedirectToAction("Login", "UserLogin");
            }else{
                var statuses = await PopulateStatusList();
            ViewData["StatusList"] = statuses;

            var tickets = new List<TicketDetail>();

            var categories = await PopulateCategoryList();
            ViewData["CategoryList"] = categories;

            var role = HttpContext.Session.GetString("Role");
            if(role == "Student")
            {
                //call api to get student tickets
                tickets = sharedClient.GetFromJsonAsync<List<TicketDetail>>($"ticket/userTickets?userId={userId}").Result;                          
            }
            else if(role == "Staff")
            {
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
            
            
        }

        private async Task<List<string>> PopulateStatusList()
        {
            var statusList = await sharedClient.GetFromJsonAsync<List<string>>("ticket/ticketstatuses");
            statusList.Insert(0, "All");
            return statusList;
        }

        private async Task<List<string>> PopulateCategoryList()
        {
            var categoryList = await sharedClient.GetFromJsonAsync<List<string>>("category/getcategoryNames");
            categoryList.Insert(0, "All");
            return categoryList;
        }

        public async Task<IActionResult> Filter(string startDate, string endDate, string status, string category)
        {
            var statuses = await PopulateStatusList();
            ViewData["StatusList"] = statuses;

            var categories = await PopulateCategoryList();
            ViewData["CategoryList"] = categories;

            //check if all the fields have been left as default
            if (startDate == null && endDate == null && status == "All" && category == "All")
            {
                return RedirectToAction("ViewTicket", "Ticket");
            }


            //get the role
            var role = HttpContext.Session.GetString("Role");
            Console.WriteLine(role);

            //if the user is an employee
            if (role == "Staff")
            {
               var devIdString = HttpContext.Session.GetInt32("DevId").ToString();
               Console.WriteLine(devIdString);
               var filteredTickets = sharedClient.GetFromJsonAsync<List<TicketDetail>>($"ticket/filter?startDate={startDate}&endDate={endDate}&status={status}&category={category}&userId={devIdString}&userRole={role}").Result; 
               return View("ViewTicket",filteredTickets);
            }
            else if (role == "Student")
            {
                var studentIdString = HttpContext.Session.GetInt32("UserId").ToString();
                var filteredTickets = sharedClient.GetFromJsonAsync<List<TicketDetail>>($"ticket/filter?startDate={startDate}&endDate={endDate}&status={status}&category={category}&userId={studentIdString}&userRole={role}").Result;               
                //redirect to view ticket action and pass the filtered tickets
                return View("ViewTicket",filteredTickets);
            }
          
            return RedirectToAction("ViewTicket", "Ticket");

        }
        

        

}