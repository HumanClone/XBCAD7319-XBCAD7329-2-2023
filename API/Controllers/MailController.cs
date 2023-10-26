using api.email;
using api.Models;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NuGet.Protocol;
using System.Reflection.Metadata;
using System.Runtime.Intrinsics.Arm;
using System.Text.RegularExpressions;

namespace api.Controllers
{
    //api/mail/method
    [ApiController]
    [Route("api/[controller]")]
    public class MailController : ControllerBase
    {
        private readonly IMailService mailService;

        private readonly XbcadDb2Context _context;

        public MailController(IMailService mailService,XbcadDb2Context context)
        {
            this.mailService = mailService;
            _context = context;
        }

        //calls the send method to send the email 
        [HttpPost("send")]
        public async Task<IActionResult> SendMailUser([FromBody]MailRequest request)// might need to be from body 
        {
            try
            {
                await mailService.SendEmailUser(request);

                return Ok();
            }
            catch (Exception ex)
            {

                throw;
            }
                
        }


        [HttpPost("adminSend")]
        public async Task<IActionResult> SendMailAdmin([FromBody]MailRequest request)//might need to be from Body
        {
            try
            {
                await mailService.SendEmailAdmin(request);
                return Ok();
            }
            catch (Exception ex)
            {

                throw;
            }
                
        }
        
        //simple get to make sure the endpoint was correct 
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                // Replace this with your logic to fetch data or perform any operation
                var data = new { Message = "Hello from API!" };
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }


     
        // gets the information from teh json this will most likely be used by the logic app 
        [HttpPost("resAT")]
        public async Task<IActionResult> ReceiveEmail([FromForm] MailReceive mailReceive)
        {
            try
            {
                // Process form fields (ToEmail, Subject, Body)
                // Handle attachments (List<IFormFile> attachments)
                string updatedBody = Regex.Replace(mailReceive.Body, "<.*?>", string.Empty);
                updatedBody = updatedBody.Replace("\\r\\n", " ");
                updatedBody = updatedBody.Replace(@"&nbsp;", " ");

                TicketResponse tr= new TicketResponse();
                var ids=_context.TicketDetails.Select(S=>S.TicketId);
                string pID=(mailReceive.Subject.StartsWith("Re:"))? mailReceive.Subject.Substring(2):mailReceive.Subject;
                bool posID=int.TryParse(pID,out int result);
                bool exist=ids.Contains(result);
                if(!posID && !exist)
                {
                    TicketDetail td=new TicketDetail();
                    td.DateIssued=DateTime.UtcNow;
                    td.MessageContent=updatedBody;
                    td.Status="Needs attention";
                    td.Priority=(int)Priority.Low;
                    _context.Add(td);
                    await _context.SaveChangesAsync();
                    var tic=_context.TicketDetails.OrderBy(s=>s.TicketId).LastOrDefault();
                    tr.TicketId=tic.TicketId.ToString();
                    
                    Console.WriteLine(td.ToString());
                }
                else
                {
                    
                    tr.TicketId=(mailReceive.Subject.StartsWith("Re:"))? mailReceive.Subject.Substring(3):mailReceive.Subject;
                }

                
                    Console.WriteLine("Attachments Recieved");
                    Console.WriteLine(mailReceive.links);
                
                


                tr.ResponseMessage=mailReceive.Body;
                tr.Sender=mailReceive.FromEmail;
                tr.Date=DateTime.UtcNow;
                _context.Add(tr);
                await _context.SaveChangesAsync();
                Console.WriteLine(tr.ToString());
                
                // Return a success response
                return Ok("Email received and processed successfully and ticket created.");
            }
            catch (Exception ex)
            {
                // Handle any errors
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }


         //TODO:Test too see if new logic app works
        [HttpPost("res")]
        public async Task<IActionResult> ReceiveEmailnoAT([FromForm] MailReceive mailReceive)
        {
            try
            {
                // Process form fields (ToEmail, Subject, Body)
                // Handle attachments (List<IFormFile> attachments)
                string updatedBody = Regex.Replace(mailReceive.Body, "<.*?>", string.Empty);
                updatedBody = updatedBody.Replace("\\r\\n", " ");
                updatedBody = updatedBody.Replace(@"&nbsp;", " ");

                TicketResponse tr= new TicketResponse();
                var ids=_context.TicketDetails.Select(S=>S.TicketId);
                string pID=(mailReceive.Subject.StartsWith("Re:"))? mailReceive.Subject.Substring(2):mailReceive.Subject;
                bool posID=int.TryParse(pID,out int result);
                bool exist=ids.Contains(result);
                if(!posID && !exist)
                {
                    TicketDetail td=new TicketDetail();
                    td.DateIssued=DateTime.UtcNow;
                    td.MessageContent=updatedBody;
                    td.Status="Needs attention";
                    td.Priority=(int)Priority.Low;
                    _context.Add(td);
                    await _context.SaveChangesAsync();
                    var tic=_context.TicketDetails.OrderBy(s=>s.TicketId).LastOrDefault();
                    tr.TicketId=tic.TicketId.ToString();
                    Console.WriteLine(td.ToString());
                }
                else
                {
                    tr.TicketId=(mailReceive.Subject.StartsWith("Re:"))? mailReceive.Subject.Substring(3):mailReceive.Subject;
                }

                


                tr.ResponseMessage=mailReceive.Body;
                tr.Sender=mailReceive.FromEmail;
                tr.Date=DateTime.UtcNow;
                _context.Add(tr);
                await _context.SaveChangesAsync();
                Console.WriteLine(tr.ToString());
                
                // Return a success response
                return Ok("Email received and processed successfully and ticket created.");
            }
            catch (Exception ex)
            {
                // Handle any errors
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }


        


        [HttpPost("sendAT")]
        public async Task<IActionResult> SendMail([FromForm]MailRequest request)
        {
            try
            {
                await mailService.SendEmailAdmin(request);
                // // TicketResponse tr= new TicketResponse();
                // // tr.ResponseMessage=request.Body;
                // // tr.TicketId=(request.Subject.StartsWith("Re:"))? request.Subject.Substring(3):request.Subject;
                // // tr.DevId=request.DevId;
                // // tr.date=DateTime.UtcNow;
                // if(!request.Attachments.IsNullOrEmpty())
                // {
                //     string links=await mailService.StoreAttachments(request.Attachments);
                //     tr.links=links;
                // }
                // // _context.Add(tr);
                // // await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex);
                return BadRequest();

            }

        }


    }    
}