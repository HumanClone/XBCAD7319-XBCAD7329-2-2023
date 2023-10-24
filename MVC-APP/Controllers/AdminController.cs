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
        // TODO REPLACE WHEN DONE!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!1
        BaseAddress = new Uri("http://localhost:5173/api/"),
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

    public async Task<IActionResult> Filter(string startDate, string endDate, string status, string category)
        {
            var statuses = await PopulateStatusList();
            ViewData["StatusList"] = statuses;

            var categories = await PopulateCategoryList();
            ViewData["CategoryList"] = categories;

            //check if all the fields have been left as default
            if (startDate == null && endDate == null && status == "All" && category == "All")
            {
                return RedirectToAction("ViewAdminTicket", "Admin");
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
              return RedirectToAction("ViewAdminTicket", "Admin");
            }
            else if (role == "Student")
            {
                var studentIdString = HttpContext.Session.GetInt32("UserId").ToString();
                var filteredTickets = sharedClient.GetFromJsonAsync<List<TicketDetail>>($"ticket/filter?startDate={startDate}&endDate={endDate}&status={status}&category={category}&userId={studentIdString}&userRole={role}").Result;               
                //redirect to view ticket action and pass the filtered tickets
                return RedirectToAction("ViewAdminTicket", "Admin");
            }
          
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
}