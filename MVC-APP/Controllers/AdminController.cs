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


namespace mvc_app.Controllers;

public class AdminController : Controller
{
   private static HttpClient sharedClient = new()
    {
        BaseAddress = new Uri("https://supportsystemapi.azurewebsites.net/api/"),
    };


    public IActionResult Index()
    {
        return View();
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
                jsonTicketContent= await response.Content.ReadAsStringAsync();
                ticketList = JsonConvert.DeserializeObject<List<TicketDetail>>(jsonTicketContent);
                ticketList = ticketList.OrderByDescending(t => t.DateIssued).ToList();
                Console.WriteLine("Pull success");
                int num=0;
            
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
            string jsonTicketContent;
            Console.WriteLine(devId);
            Console.WriteLine(ticketId);
        
            var response = await sharedClient.GetAsync("ticket/ticket?ticketId="+ticketId);
            var ticket = await response.Content.ReadFromJsonAsync<TicketDetail>();

            ticket.DevId = devId;
            ticket.Status = "In Progress"; 

            var res = await sharedClient.PostAsJsonAsync("ticket/editTicket", ticket);
            Console.WriteLine(res.StatusCode);

            var mail = new MailRequest();
            mail.ToEmail = "yusraadnan24@gmail.com";
            mail.Subject = ticketId;
            mail.Body = "Hello "+ mail.ToEmail + " you have been assigned for responding to the ticket";
            var re= await sharedClient.PostAsJsonAsync("mail/adminSend", mail);
           
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