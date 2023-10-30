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
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing.Template;
using MVCAPP.Templates;


namespace mvc_app.Controllers;

public class ResponseController : Controller
{
     private static HttpClient sharedClient = new()
     {
         BaseAddress = new Uri("https://supportsystemapi.azurewebsites.net/api/"),
     };

    //  private static HttpClient sharedClient = new()
    //  {
    //      BaseAddress = new Uri("http://localhost:5173/api/"),
    //  };

    [HttpGet]
    public IActionResult Create(string? id)
    {
        ViewBag.TicketID = id;
        return View();
    }



    [HttpGet]
    public async Task<IActionResult> Index(string? id, string? selectTemplate, string? notes, string? responseBody)
    {
        Console.WriteLine(id);
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

                var ticket = await sharedClient.GetAsync("ticket/ticket?ticketID="+id);
                var viewTicket = await ticket.Content.ReadFromJsonAsync<TicketDetail>();

                ResponseViewModel res = new ResponseViewModel();
                res.Ticket = viewTicket;
                res.Responses = responses;

                if(viewTicket.Links!=null){
                    res.Links = viewTicket.Links.Split(";").ToList();
                } 

                foreach(var userResponse in responses)
                {
                    if(userResponse.DevId!=null){
                        var devEmail = await sharedClient.GetAsync("users/devEmail?devId="+userResponse.DevId);
                        var email = await devEmail.Content.ReadAsStringAsync();
                        userResponse.sender = email;                      
                    }else if(userResponse.sender!=null){
                        var userEmail = await sharedClient.GetAsync("users/user?userId="+userResponse.sender);
                        var user = await userEmail.Content.ReadFromJsonAsync<UserInfo>();
                        var email = user.Email;
                        userResponse.sender = email;
                    }
                }               


                if(HttpContext.Session.GetInt32("DevId")!=null){
                    var priority = await sharedClient.GetAsync("ticket/getPriorityName?priority="+viewTicket.Priority);
                    var priorityName = await priority.Content.ReadAsStringAsync();
                    res.Priority = priorityName;

                    var templateService = new TemplateService();
                    var templateNames = templateService.GetTemplateNames();

                    ViewData["TemplateNames"] = templateNames;
               
                    ViewData["Template"] = selectTemplate;

                    if(viewTicket.Notes == null){
                        viewTicket.Notes = "";
                    }else if(notes != null){
                        viewTicket.Notes = notes;
                    }

                    if(responseBody != null){
                        ViewData["Template"] = responseBody;
                    }
                }
                

                try
                {
                    var send=responses.Where(s=>s.sender!=null).Select(s=>s.sender).FirstOrDefault();
                    HttpContext.Session.SetString("Send",send);
                    Console.WriteLine(send);
                }
                catch(System.ArgumentNullException ex)
                {
                    return View(res);
                }
                catch (System.InvalidOperationException ex)
                {
                    return View(res);
                }           

                return View(res);
                
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

    //author:Code maze

    //https://code-maze.com/aspnetcore-multipart-form-data-in-httpclient/
    public async Task<IActionResult> SendResponseNew(string ticketId,string body,List<IFormFile> files)
    {
        ViewBag.TicketID=ticketId;
        Mail model=new Mail();
        model.Subject="Re:"+ticketId;
        model.Body=body;
        
        if(HttpContext.Session.GetInt32("UserId")==null)
        {
            var res= await sharedClient.GetAsync("ticket/ticket?ticketId="+ticketId);
            var ticket=await res.Content.ReadFromJsonAsync<TicketDetail>();
            HttpResponseMessage respo = await sharedClient.GetAsync("response/ticket/?ticketID="+ticketId);
            List<TicketResponse> resp = await respo.Content.ReadFromJsonAsync<List<TicketResponse>>();
            var send=resp.Where(s=>s.sender!=null).Select(s=>s.sender).FirstOrDefault();

            if(ticket.UserId==null)
            {
                Console.WriteLine("Not from system");
                model.Toemail=send;
            }
            else
            {
                var re= await sharedClient.GetAsync("users/user?userId="+ticket.UserId);
                var user=await re.Content.ReadFromJsonAsync<UserInfo>();
                var email=user.Email;
                Console.WriteLine("Email is "+ email);
                model.Toemail=email;
            }
        
            Console.WriteLine("Dev");
            Console.WriteLine(HttpContext.Session.GetInt32("DevId"));
            Console.WriteLine(model.Subject);
            Console.WriteLine(model.Toemail);
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
                    return RedirectToAction("Index", "Response", new { id = ticketId});
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
                return RedirectToAction("MyTickets","Dev");
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
                    return RedirectToAction("Index", "Response", new { id = ticketId});
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


    public async Task<IActionResult> LoadTemplate(string selectTemplate,string ticketId,string? notes)
    {
        var templateService = new TemplateService();
        var chosenTemplate = templateService.GetTemplateContent(selectTemplate);;
        return RedirectToAction("Index", "Response", new { id = ticketId, selectTemplate = chosenTemplate, notes = notes });
    }

    public async Task<IActionResult> AddNotes(string notes, string ticketId, string? response)
    {
        var ticket = await sharedClient.GetAsync("ticket/ticket?ticketID=" + ticketId);
        var viewTicket = await ticket.Content.ReadFromJsonAsync<TicketDetail>();
        viewTicket.Notes = notes;
    
        var jsonContent = JsonConvert.SerializeObject(viewTicket);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
    
        var editResponse = await sharedClient.PostAsync("ticket/editTicket", content);
    
        if (editResponse.IsSuccessStatusCode)
        {
            Console.WriteLine("Notes updated successfully");
            return RedirectToAction("Index", "Response", new { id = ticketId, notes = notes, responseBody = response });
        }
        else
        {
            Console.WriteLine($"Request failed with status code: {editResponse.StatusCode}");
            return RedirectToAction("MyTickets", "Dev");
        }
    }


}
