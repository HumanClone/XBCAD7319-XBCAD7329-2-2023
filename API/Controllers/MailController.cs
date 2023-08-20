
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using api.email;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MailController : ControllerBase
    {
        private readonly IMailService mailService;

        public MailController(IMailService mailService)
        {
            this.mailService = mailService;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendMail([FromForm]MailRequest request)
        {
            try
            {
                await mailService.SendEmailAsync(request);
                //method to add it to the database
                return Ok();
            }
            catch (Exception ex)
            {

                throw;
            }
                
        }


        [HttpPost]
        public async Task<IActionResult> ReceiveEmailWithAttachments(
        [FromForm] string FromEmail,
        [FromForm] string Subject,
        [FromForm] string Body,
        [FromForm] string ReceivedDate,
        [FromForm(Name = "Attachment")] List<IFormFile> attachments)
        {
            try
            {
                // Process form fields (ToEmail, Subject, Body)
                // Handle attachments (List<IFormFile> attachments)

                // Create the MailReceive object
                var req = new MailReceive
                {
                    FromEmail = FromEmail,
                    Body = Body,
                    Subject = Subject,
                    ReceivedDate = DateTime.Parse(ReceivedDate),
                    Attachments = attachments
                };

                // Now you can use 'req' to add contents to the database
                // Store attachments in the database or other appropriate storage

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