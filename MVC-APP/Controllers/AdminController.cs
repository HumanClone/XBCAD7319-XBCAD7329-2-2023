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

            var priority=PopulatePri();
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
                    if(!ticket.Status.Equals("closed"))
                    {
                        bool isCloseToPriorityAllowance = checkPriority(ticket);

                        if (isCloseToPriorityAllowance == true)
                        {
                            // Notify the dev that the ticket is close to its priority allowance
                            _notyf.Warning($"Ticket {ticket.TicketId} is close to its priority allowance.");
                        }
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

             var priority= PopulatePri();
             ViewData["Priority"]=priority;
            

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
            string JsonDevContent;
            Console.WriteLine(devId);
            Console.WriteLine(ticketId);
        
            List<TeamDev> DevList = new List<TeamDev>();
            var response = await sharedClient.GetAsync("ticket/ticket?ticketId="+ticketId);
            var ticket = await response.Content.ReadFromJsonAsync<TicketDetail>();

            ticket.DevId = devId;
            ticket.Status = "In Progress"; 

            var res = await sharedClient.PostAsJsonAsync("ticket/editTicket", ticket);
            Console.WriteLine(res.StatusCode);

            HttpResponseMessage devResponse = await sharedClient.GetAsync("Users/devs");
            if(devResponse.IsSuccessStatusCode)
            {
                JsonDevContent= await devResponse.Content.ReadAsStringAsync();
                DevList = JsonConvert.DeserializeObject<List<TeamDev>>(JsonDevContent);
                Console.WriteLine("Pull of devs success", DevList);
            }
            else
            {
                Console.WriteLine("Pull of devs failed");
            }
            var DevEmail = DevList.Where(dev => dev.DevId == int.Parse(devId)).Select(dev => dev.Email).FirstOrDefault();
            Console.WriteLine(DevEmail);
            var mail = new MailRequest();
            mail.ToEmail = DevEmail;
            mail.Subject = ticketId;
            mail.Body = "Hello "+ mail.ToEmail + " you have been assigned for responding to ticket no." + mail.Subject;
            var re= await sharedClient.PostAsJsonAsync("mail/adminSend", mail);
           
            return RedirectToAction("ViewAdminTicket", "Admin");


        }

        private List<SelectListItem> PopulatePri()
         {

            List<SelectListItem> selectListItems= new List<SelectListItem>();
            selectListItems.Add(new SelectListItem(value: "0", text: "Low")); 
            selectListItems.Add(new SelectListItem(value: "1", text:"Medium")); 
            selectListItems.Add(new SelectListItem(value: "2", text: "High")); 
            selectListItems.Add(new SelectListItem(value: "3", text: "Very High")); 
            return selectListItems;

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



        public async Task<IActionResult> Search(string ticketId)
        {
            var statuses = await PopulateStatusList();
            ViewData["StatusList"] = statuses;

            var categories = await PopulateCategoryList();
            ViewData["CategoryList"] = categories;

             var emails=await PopulateEmails();
             ViewData["Emails"]=emails;
            Console.WriteLine(ticketId);
            try
            {
                var ticket= await  sharedClient.GetFromJsonAsync<TicketDetail>("ticket/ticket?ticketId="+ticketId);
                List<TicketDetail> tickets= new List<TicketDetail>();
                tickets.Add(ticket); 
                
                return View("ViewAdminTicket",tickets);
            }
            catch(Exception ex)
            {
                var tickets = new List<TicketDetail>();
                return View("ViewAdminTicket",tickets);
            }

        }

        public async Task<IActionResult> PriAs (string priority, string ticketId)
        {
           string JsonDevContent; 

           Console.WriteLine(priority); 
           Console.WriteLine(ticketId);

           List<TeamDev> DevList = new List<TeamDev>();

           var response = await sharedClient.GetAsync("ticket/ticket?ticketId="+ticketId);

           var ticket = await response.Content.ReadFromJsonAsync<TicketDetail>();

           ticket.Priority=int.Parse(priority); 

           var res = await sharedClient.PostAsJsonAsync("ticket/editTicket", ticket); 
           Console.WriteLine(res.StatusCode);

           return RedirectToAction ("ViewAdminTicket", "Admin");
        }
}