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

        public TicketController(StudentSupportXbcadContext context)
        {
            _context = context;
            
        }

        //TODO:endpoint to add a ticket to the db 
        [HttpPost("create")]
        public async Task<IActionResult> createTicket([FromBody]TicketDetail ticket)
        {
            return null;
        }

        //TODO:endpoint to return a list of tickets 
        [HttpGet("tickets")]
        public async Task<IActionResult> getTickets()
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

        //TODO:^^ above ones with date fields for criteria


        //TODO:end point to edit a ticket
        [HttpPost("editTicket")]
        public async Task<IActionResult> editTicket(string? ticketID,[FromBody] TicketDetail ticket)
        {
            return null;
        }

        //TODO:endpoint to close a ticket
        [HttpPost("closeTicket")]
        public async Task<IActionResult> closeTicket(string? ticketID,[FromBody] TicketDetail ticket)
        {
            return null;
        }
        

    }


}
