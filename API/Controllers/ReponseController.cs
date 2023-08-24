using api.email;
using api.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ResponseController:ControllerBase
    {
        private readonly StudentSupportXbcadContext _context;
        private readonly IMailService mailService;


        public ResponseController(StudentSupportXbcadContext context,IMailService mailService)
        {
            _context = context;
            this.mailService = mailService;
            
        }

        //TODO:endpoint to get all responses from everyticket 
        [HttpGet("all")]
        public async Task<IActionResult> getResponses()
        {
            return null;
        }


        //TODO:endpoint to get all responses for a ticket 
        [HttpGet("ticket")]
        public async Task<IActionResult> getResponsesTicket(string? ticketID)
        {
            return null;
        }


        //TODO:calls the send method to send the email 
        [HttpPost("sendAdmin")]
        public async Task<IActionResult> SendMail([FromForm]MailRequest request)
        {
            try
            {
                await mailService.SendEmailAdmin(request);
                //method to add it to the database
                return Ok();
            }
            catch (Exception ex)
            {

                throw;
            }
                
        }


        //TODO:endpoint to get all responses by a dev
        [HttpGet("dev")]
        public async Task<IActionResult> getResponsesDev(string? DevID)
        {
            return null;
        }


    }
}