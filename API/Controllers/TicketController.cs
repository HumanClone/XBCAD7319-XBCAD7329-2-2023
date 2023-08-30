using api.email;
using api.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TicketController:ControllerBase
    {
        private readonly StudentSupportXbcadContext _context;
        private readonly IMailService mailService;

        public TicketController(StudentSupportXbcadContext context,IMailService mailService)
        {
            _context = context;
            this.mailService = mailService;
            
        }

        //TODO:endpoint to add a ticket to the db 
        [HttpPost("create")]
        public async Task<IActionResult> createTicket([FromBody]TicketDetail ticket)
        {
            return null;
        }


        //endpoint the creates a ticket from the user side and sends emai 
        [HttpPost("createuserticket")]
        public async Task<IActionResult> createTicketUser([FromBody]TicketDetail ticket)
        {
            _context.Add(ticket);
            await _context.SaveChangesAsync();
            var tic=_context.TicketDetails.Find(ticket);
            MailRequest req= new MailRequest();
            req.Body=tic.MessageContent;
            req.Subject=tic.TicketId.ToString();
            try
            {
                await mailService.SendEmailUser(req);
                TicketResponse tr= new TicketResponse();
                tr.ResponseMessage=req.Body;
                tr.TicketId=req.Subject;
                tr.Date=DateTime.Now;
                _context.Add(tr);
                await _context.SaveChangesAsync();

                return Ok("It worked");
            }
            catch (Exception ex)
            {


                return BadRequest();

            }
                

            
        }



        //TODO:endpoint to return a list of tickets 
        [HttpGet("tickets")]
        public async Task<IActionResult> getTickets()
        {
            return null;
        }

        [HttpGet("ticket")]
        public async Task<IActionResult> getTickets(string? ticketID)
        {
            return null;
        }


        //TODO:endpoint to get tickets of a dev
        [HttpGet("devTickets")]
        public async Task<IActionResult> getDevTickets(string? DevId)
        {
            return null;
        }


        //TODO:end point to return tickets within a date range

        //TODO:return a list of tickets that are open 

        //TODO:return a list of tickets that are closed 



        //TODO:end point to edit a ticket
        [HttpPost("editTicket")]
        public async Task<IActionResult> editTicket(string? ticketID,[FromBody] TicketDetail ticket)
        {
            return null;
        }

        //endpoint to close a ticket
        [HttpPost("closeTicket")]
        public async Task<IActionResult> closeTicket(string? ticketID,[FromBody] MailRequest request)
        {
            var ticket=_context.TicketDetails.Select(s=>s).Where(s=>s.TicketId.Equals(ticketID)).Single();
            ticket.Status="closed";
            _context.Update(ticket);
            try
            {

                await mailService.SendEmailAdmin(request);
                TicketResponse tr= new TicketResponse();
                tr.ResponseMessage=request.Body;
                tr.TicketId=ticket.TicketId.ToString();
                tr.DevId=request.DevId;
                tr.Date=DateTime.Now;
                _context.Add(tr);
                await _context.SaveChangesAsync();
                
                return Ok();
            }
            catch (Exception ex)
            {


                return BadRequest();

            }
            
        }
        

    }


}
