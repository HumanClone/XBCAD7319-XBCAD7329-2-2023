using System.Net;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Json;
using MVCAPP.Models;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Intrinsics.X86;
using AspNetCoreHero.ToastNotification.Abstractions;

namespace mvc_app.Controllers;

public class AdminController : Controller
{
   private static HttpClient sharedClient = new()
    {
        BaseAddress = new Uri("https://supportsystemapi.azurewebsites.net/api/"),
    };

    private readonly INotyfService _notyf;

    public AdminController(INotyfService notyf)
    {
        _notyf = notyf;
    }


    public IActionResult Index()
    {
        return View();
    }

    public Boolean checkPriority(TicketDetail ticket)
        {
            // Calculate the expected time threshold based on the priority
            double timeThreshold = 0;

            if(ticket.Priority != null)
            {
                switch ((Priority)ticket.Priority)
                {
                    
                    case Priority.Very_High:
                        timeThreshold = 10;
                        break;
                    case Priority.High:
                        timeThreshold = 24;
                        break;
                    case Priority.Medium:
                        timeThreshold = 72;
                        break;
                    case Priority.Low:
                        timeThreshold = 168;
                        break;
                    default:
                        timeThreshold = 0;
                        break;
                }
            }

            // Calculate the time remaining before the ticket reaches the threshold
            double timeRemaining = timeThreshold - (DateTime.Now - ticket.DateIssued).Hours;

            Console.WriteLine(timeRemaining);
            if (timeRemaining < (0.1 * timeThreshold))
            {
                Console.WriteLine("Admin Priority Notify");
                return true;
            }
            else if (timeRemaining < 0)
            {
                Console.WriteLine("Admin Priority False Notify");
                return false;
            }
            Console.WriteLine("Admin Priority Screwed");
            return false;
        }

    
    //Gets all tickerts from API for the admin
      [HttpGet]
        public async Task<IActionResult> ViewAdminTicket()
        {  
            var statuses = await PopulateStatusList();
            ViewData["StatusList"] = statuses;

            var categories = await PopulateCategoryList();
            ViewData["CategoryList"] = categories;
            
            var emails=await PopulateEmails();
            ViewData["Emails"]=emails;

            string jsonTicketContent;
            List<TicketDetail> ticketList = new List<TicketDetail>();
           
            HttpResponseMessage response = await sharedClient.GetAsync("Ticket/getalltickets");
            if(response.IsSuccessStatusCode)
            {
                jsonTicketContent = await response.Content.ReadAsStringAsync();
                ticketList = JsonConvert.DeserializeObject<List<TicketDetail>>(jsonTicketContent);
                ticketList = ticketList.OrderByDescending(t => t.DateIssued).ToList();
                Console.WriteLine("Pull success");
                int num=0;
            
                foreach (var ticket in ticketList)
                {
                    bool isCloseToPriorityAllowance = checkPriority(ticket);

                    if (isCloseToPriorityAllowance == true)
                    {
                        // Notify the dev that the ticket is close to its priority allowance
                        _notyf.Warning($"Ticket {ticket.TicketId} is close to its priority allowance.");
                    }
                }
            }
            else{
                Console.WriteLine("pull failed");
            }
            return View(ticketList);
        }

    public async Task<IActionResult> Filter(string startDate, string endDate, string status, string category)
        {
            var statuses = await PopulateStatusList();
            ViewData["StatusList"] = statuses;

            var categories = await PopulateCategoryList();
            ViewData["CategoryList"] = categories;

             var emails=await PopulateEmails();
             ViewData["Emails"]=emails;
            

            //check if all the fields have been left as default
            if (startDate == null && endDate == null && status == "All" && category == "All")
            {
                return RedirectToAction("ViewAdminTicket", "Admin");
            }


            var role = "Admin";
            var devIdString = HttpContext.Session.GetInt32("DevId").ToString();
            var filteredTickets = sharedClient.GetFromJsonAsync<List<TicketDetail>>($"ticket/filter?startDate={startDate}&endDate={endDate}&status={status}&category={category}&userId={devIdString}&userRole={role}").Result; 


            return View("ViewAdminTicket", filteredTickets);

        }

        public async Task<IActionResult> Assign(string devId,string ticketId)
        {
            Console.WriteLine(devId);
            Console.WriteLine(ticketId);
            return RedirectToAction("ViewAdminTicket", "Admin");


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

        private async Task<List<SelectListItem>> PopulateEmails()
        {
            var devslist=await sharedClient.GetFromJsonAsync<List<TeamDev>>("users/devs");

            var selectListItems = devslist
            .Select(dev => new SelectListItem
            {
                Text = dev.Email,
                Value = dev.DevId.ToString()
            })
            .ToList();  
           
            return selectListItems;
        }

        
}