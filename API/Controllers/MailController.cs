using api.email;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

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

        //calls the send method to send the email 
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