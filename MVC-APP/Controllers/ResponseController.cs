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
using Newtonsoft.Json;
using Microsoft.IdentityModel.Tokens;


namespace mvc_app.Controllers;

public class ResponseController : Controller
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

    [HttpGet]
    public IActionResult Create(string? id)
    {
        ViewBag.TicketID = id;
        return View();
    }



    [HttpGet]
    public async Task<IActionResult> Index(string? id)
    {
        try
        {
            Console.WriteLine("Before Request");
            HttpResponseMessage response = await sharedClient.GetAsync("response/ticket/?ticketID="+id);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Success");
                List<TicketResponse> responses = await response.Content.ReadFromJsonAsync<List<TicketResponse>>();
                ViewBag.TicketID = id;
                Console.WriteLine("After");
                try
                {
                    var send=responses.Where(s=>s.sender!=null).Select(s=>s.sender).FirstOrDefault();
                    HttpContext.Session.SetString("Send",send);
                    Console.WriteLine(send);
                }
                catch(System.ArgumentNullException ex)
                {
                    return View(responses);
                }
                catch (System.InvalidOperationException ex)
                {
                    return View(responses);
                }
                return View(responses);
                
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



    [HttpPost]
    //[Route("/Response/mail")]
    public async Task<IActionResult> SendResponse(string subject,string body,string toemail)
    {
        Mail model=new Mail();
        model.Subject=subject;
        model.Body=body;
        model.Toemail=toemail;
        if(HttpContext.Session.GetInt32("UserId")==null)
        {
            Console.WriteLine("Dev");
            Console.WriteLine(HttpContext.Session.GetInt32("DevId"));
            Console.WriteLine(model.Subject);
            Console.WriteLine(model.Body);
            var mr = new MailRequest
            {
                
                Subject =model.Subject,
                Body = model.Body,
                DevId = ""+HttpContext.Session.GetInt32("DevId"),
                ToEmail = (model.Toemail.IsNullOrEmpty()) ? "" : model.Toemail
            
            };
            
            try
            {

                HttpResponseMessage response = await sharedClient.PostAsJsonAsync<MailRequest>("response/Admin",mr);
                
                string responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine(responseContent);
                if(response.IsSuccessStatusCode)
                {
                    var ticket = await response.Content.ReadFromJsonAsync<TicketResponse>();
                    Console.WriteLine($"Response sent {response.StatusCode}");
                    return RedirectToAction("ViewTicket","Ticket");
                }
                else
                {
                    Console.WriteLine($"Request failed with status code: {response.RequestMessage}");
                    Console.WriteLine($"Request failed with status code: {response.ToString}");

                    return View("Create");
                }
            }
            catch (System.Exception)
            {
                return RedirectToAction("ViewTicket","Ticket");
                throw;
            }
        }

        else
        {
            Console.WriteLine("User");
            var mr = new MailRequest
            {
                
                Subject =model.Subject,
                Body = model.Body,
                UserId = ""+HttpContext.Session.GetInt32("UserId"),
                ToEmail = ""
            
            };

            try
            {
                Console.WriteLine(model.Subject);
                Console.WriteLine(model.Body);
                Console.WriteLine(HttpContext.Session.GetInt32("UserId")+"");

                HttpResponseMessage response = await sharedClient.PostAsJsonAsync("response/sendUser", mr);

                if(response.IsSuccessStatusCode)
                {
                    var ticket = await response.Content.ReadFromJsonAsync<TicketResponse>();
                    Console.WriteLine($"Response sent {response.StatusCode}");
                    return RedirectToAction("ViewTicket","Ticket");
                }
                else
                {
                    Console.WriteLine($"Request failed with status code: {response.StatusCode}");
                    return View("Create");
                }
            }
            catch (System.Exception)
            {
                return RedirectToAction("ViewTicket","Ticket");
                throw;
            }
           
        }
    }
}
