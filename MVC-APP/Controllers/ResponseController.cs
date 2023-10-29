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


namespace mvc_app.Controllers;

public class ResponseController : Controller
{
    //  private static HttpClient sharedClient = new()
    //  {
    //      BaseAddress = new Uri("https://supportsystemapi.azurewebsites.net/api/"),
    //  };

     private static HttpClient sharedClient = new()
     {
         BaseAddress = new Uri("https://supportsystemapi.azurewebsites.net/api/"),
     };

    [HttpGet]
    public IActionResult Create(string? id)
    {
        ViewBag.TicketID = id;
        ViewData["Temp"]=PopulateTemplates();
        return View();
    }



    [HttpGet]
    public async Task<IActionResult> Index(string? id)
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




    public async Task<IActionResult> LoadTemplate(string choice,string ticketId)
    {
        Console.WriteLine(ticketId);
        Console.WriteLine(choice);
        
        ViewBag.TicketID = ticketId;
        ViewData["Temp"]=PopulateTemplates();
        ViewData["Body"]=getTemplate(choice);
        return View("Create");
    }

    public List<SelectListItem> PopulateTemplates()
    {
        var templates=new List<SelectListItem>();
        templates.Add(new SelectListItem(value:"yo",text:"First Option"));
        return templates;
    }

    public string getTemplate(string value)
    {
        Console.WriteLine("get");
        string text="";
        switch(value)
        {
            case"yo":text="This is a template response";break;
        }
        return text;

    }





}


}

