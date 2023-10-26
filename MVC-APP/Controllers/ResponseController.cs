using System.Net;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Json;
using MVCAPP.Models;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.IdentityModel.Tokens;
using System.Net.Mime;
using Microsoft.AspNetCore.Http.Features;


namespace mvc_app.Controllers;

public class ResponseController : Controller
{
     private static HttpClient sharedClient = new()
     {
         BaseAddress = new Uri("https://supportsystemapi.azurewebsites.net/api/"),
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
    //TODO:GET the Files
    //https://code-maze.com/aspnetcore-multipart-form-data-in-httpclient/
    public async Task<IActionResult> SendResponse(string subject,string body,string toemail,List<IFormFile> files)
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
            Console.WriteLine(files.Count.ToString());
            var mr = new MailRequest
            {
                Subject =model.Subject,
                Body = model.Body,
                DevId = ""+HttpContext.Session.GetInt32("DevId"),
                ToEmail = (model.Toemail.IsNullOrEmpty()) ? "." : model.Toemail,
                Attachments=files
            };

            using MultipartFormDataContent multipartContent = new();
            multipartContent.Add(new StringContent(mr.Subject,Encoding.UTF8, MediaTypeNames.Text.Plain),"Subject");
            multipartContent.Add(new StringContent(mr.Body,Encoding.UTF8, MediaTypeNames.Text.Plain),"Body");
            if(!mr.ToEmail.IsNullOrEmpty())
            {
                multipartContent.Add(new StringContent(mr.ToEmail,Encoding.UTF8, MediaTypeNames.Text.Plain),"ToEmail");
            }
            
            multipartContent.Add(new StringContent(mr.DevId,Encoding.UTF8, MediaTypeNames.Text.Plain),"DevId");

            if(!mr.Attachments.IsNullOrEmpty())
            {
                foreach(var file in mr.Attachments)
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
            }
                foreach(var item in multipartContent)
                {
                    Console.WriteLine(item.Headers.ToString());
                }

            try
            {
                Console.WriteLine("before request");
                var response = await sharedClient.PostAsync("response/Admin",multipartContent);
                
                string responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine(responseContent);
                if(response.IsSuccessStatusCode)
                {
                    Console.WriteLine($": {response.RequestMessage.ToString()}");

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
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
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
                ToEmail = " ",
                Attachments=files
            
            };
            Console.WriteLine("object created");
            using MultipartFormDataContent multipartContent = new();
            multipartContent.Add(new StringContent(mr.Subject,Encoding.UTF8, MediaTypeNames.Text.Plain),"Subject");
            multipartContent.Add(new StringContent(mr.Body,Encoding.UTF8, MediaTypeNames.Text.Plain),"Body");
            multipartContent.Add(new StringContent(mr.UserId,Encoding.UTF8, MediaTypeNames.Text.Plain),"UserId");

            if(!mr.Attachments.IsNullOrEmpty())
            {
                foreach(var file in mr.Attachments)
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
                Console.WriteLine(model.Subject);
                Console.WriteLine(model.Body);
                Console.WriteLine(HttpContext.Session.GetInt32("UserId")+"");

                var response = await sharedClient.PostAsync("response/sendUser", multipartContent);

                if(response.IsSuccessStatusCode)
                {
                    //var ticket = await response.Content.ReadFromJsonAsync<TicketResponse>();
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
