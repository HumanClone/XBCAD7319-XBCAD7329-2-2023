
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


        [HttpPost("res")]
        public async Task<IActionResult> ReceiveEmailWithAttachments([FromBody] MailReceive mailReceive)
        {
            try
            {
                // Process form fields (ToEmail, Subject, Body)
                // Handle attachments (List<IFormFile> attachments)

                // Create the MailReceive object
                var tempObject = new MailReceive
                {
                    FromEmail = mailReceive.FromEmail,
                    Subject = mailReceive.Subject,
                    Body = mailReceive.Body,
                    ReceivedDate = mailReceive.ReceivedDate,
                    Attachments = mailReceive.Attachments
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