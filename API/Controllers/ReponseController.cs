using api.email;
using api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Text.RegularExpressions;

namespace api.Controllers
{


//var blobServiceClient = new BlobServiceClient(connectionString);
    [ApiController]
    [Route("api/[controller]")]
    public class ResponseController:ControllerBase
    {
        private readonly XbcadDb2Context _context;
        private readonly IMailService mailService;


        public ResponseController(XbcadDb2Context context,IMailService mailService)
        {
            _context = context;
            this.mailService = mailService;
            
        }


        //endpoint to get all responses from everyticket 
        [HttpGet("all")]
        public async Task<List<TicketResponse>> getResponses()
        {
            var data=_context.TicketResponses.ToList();
            return data;
        }


        //endpoint to get all responses for a ticket 
        [HttpGet("ticket")]
        public async Task<List<TicketResponse>> getResponsesTicket(string? ticketID)
        {
            var data=_context.TicketResponses.ToList();
            data=data.Select(s=>s).Where(s=>s.TicketId.Equals(ticketID)).ToList();
            return data;
        }


        //calls the send method to send the email 
        //TODO:TEst
        [HttpPost("Admin")]
        public async Task<IActionResult> SendMail([FromForm]MailRequest request)
        {
            if(request.ToEmail.Equals("."))
            {
                request.ToEmail=null;
            }
            try
            {
                await mailService.SendEmailAdmin(request);
                TicketResponse tr= new TicketResponse();
                tr.ResponseMessage=request.Body;
                tr.TicketId=(request.Subject.StartsWith("Re:"))? request.Subject.Substring(3):request.Subject;
                tr.DevId=request.DevId;
                tr.Date=DateTime.UtcNow;
                if(!request.Attachments.IsNullOrEmpty())
                {
                    Console.WriteLine("Admin attachments");
                    string links=await mailService.StoreAttachments(request.Attachments);
                    //tr.links=links;
                }


                _context.Add(tr);
                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return BadRequest();

            }
                
        }

        //TODO:TEst
        [HttpPost("sendUser")]
        public async Task<IActionResult> SendMailuser([FromForm]MailRequest request)
        {
            try
            {
                await mailService.SendEmailUser(request);
                TicketResponse tr= new TicketResponse();
                tr.ResponseMessage=request.Body;
                tr.TicketId=(request.Subject.StartsWith("Re:"))? request.Subject.Substring(3):request.Subject;
                tr.Sender=request.UserId;
                tr.Date=DateTime.UtcNow;
                if(!request.Attachments.IsNullOrEmpty())
                {
                    Console.WriteLine("User attachments");
                    string links=await mailService.StoreAttachments(request.Attachments);
                    //tr.links=links;
                }
                

                _context.Add(tr);
                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex);
                return BadRequest();

            }
                
        }



        //endpoint to get all responses by a dev
        [HttpGet("dev")]
        public async Task<List<TicketResponse>> getResponsesDev(string? DevID)
        {
            var data=_context.TicketResponses.Select(s=>s).Where(s=>s.DevId.Equals(DevID)).ToList();
            return data;

        }

        [HttpGet("devTicket")]
        public async Task<List<TicketResponse>> getResponsesDev(string? DevID,string? ticketID)
        {
            var data=_context.TicketResponses.Select(s=>s).Where(s=>s.DevId.Equals(DevID)&& s.TicketId.Equals(ticketID)).ToList();
            return data;

        }


    }
}