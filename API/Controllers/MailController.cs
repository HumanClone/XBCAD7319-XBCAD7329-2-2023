using api.email;
using api.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace api.Controllers
{

    //api/mail/method
    [ApiController]
    [Route("api/[controller]")]
    public class MailController : ControllerBase
    {
        private readonly IMailService mailService;
        private readonly StudentSupportXbcadContext _context;

        public MailController(IMailService mailService,StudentSupportXbcadContext context)
        {
            this.mailService = mailService;
            _context = context;
        }

        //calls the send method to send the email 
        [HttpPost("send")]
        public async Task<IActionResult> SendMailUser([FromForm]MailRequest request)
        {
            try
            {
                await mailService.SendEmailUser(request);
                TicketResponse tr= new TicketResponse();
                tr.ResponseMessage=request.Body;
                tr.TicketId=(request.Subject.StartsWith("Re:"))? request.Subject.Substring(3):request.Subject;
                //tr.DevId=request.sender;
                //tr.date=DateTime.Now();

                //method to add it to the database
                return Ok();
            }
            catch (Exception ex)
            {

                throw;
            }
                
        }


        [HttpPost("adminSend")]
        public async Task<IActionResult> SendMailAdmin([FromForm]MailRequest request)
        {
            try
            {
                await mailService.SendEmailAdmin(request);
                TicketResponse tr= new TicketResponse();
                tr.ResponseMessage=request.Body;
                tr.TicketId=(request.Subject.StartsWith("Re:"))? request.Subject.Substring(3):request.Subject;
                tr.DevId=request.DevId;
                //tr.date=DateTime.Now();
                _context.Add(tr);
                await _context.SaveChangesAsync();
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
        [HttpPost("res")]
        public async Task<IActionResult> ReceiveEmailWithAttachments([FromBody] MailReceive mailReceive)
        {
            try
            {
                // Process form fields (ToEmail, Subject, Body)
                // Handle attachments (List<IFormFile> attachments)
                string updatedBody = Regex.Replace(mailReceive.Body, "<.*?>", string.Empty);
                updatedBody = updatedBody.Replace("\\r\\n", " ");
                updatedBody = updatedBody.Replace(@"&nbsp;", " ");

                // Create the MailReceive object
                var tempObject = new MailReceive
                {
                    FromEmail = mailReceive.FromEmail,
                    Subject = mailReceive.Subject,
                    Body =updatedBody,
                    ReceivedDate = mailReceive.ReceivedDate,
                    //Attachments = mailReceive.Attachments
                };

                TicketResponse tr= new TicketResponse();
                tr.ResponseMessage=mailReceive.Body;
                tr.TicketId=(mailReceive.Subject.StartsWith("Re:"))? mailReceive.Subject.Substring(3):mailReceive.Subject;
                //tr.sender=mailReceive.FromEmail;
                //tr.name=DateTime.Now();

                _context.Add(tr);
                await _context.SaveChangesAsync();


                // Return a success response
                return Ok("Email received and processed successfully.");
            }
            catch (Exception ex)
            {
                // Handle any errors
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

    
    }
}