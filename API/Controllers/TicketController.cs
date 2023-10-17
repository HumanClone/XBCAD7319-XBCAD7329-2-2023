using api.email;
using api.Models;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

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

        //endpoint to add a ticket to the db 
        [HttpPost("create")]
        public async Task<IActionResult> createTicket([FromBody]TicketDetail ticket)
        {
            try
            {
                _context.Add(ticket);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch(Exception ex)
            {
                return BadRequest();
            }
            
        }


        //endpoint the creates a ticket from the user side and sends email 
        // [HttpPost("createuserticket")]
        // public async Task<IActionResult> createTicketUser([FromBody]TicketDetail ticket)
        // {
        //     _context.Add(ticket);
        //     await _context.SaveChangesAsync();
        //     var tic=_context.TicketDetails.OrderBy(s=>s.TicketId).LastOrDefault();
        //     MailRequest req= new MailRequest();
        //     req.Body=tic.MessageContent;
        //     req.Subject=tic.TicketId.ToString();
        //     try
        //     {
        //         await mailService.SendEmailUser(req);
        //         TicketResponse tr= new TicketResponse();
        //         tr.ResponseMessage=req.Body;
        //         tr.sender=tic.UserId.ToString();
        //         tr.TicketId=req.Subject;
        //         tr.date=DateTime.UtcNow;
        //         _context.Add(tr);
        //         await _context.SaveChangesAsync();


        //         return Ok(tic);

        //     }
        //     catch (Exception ex)
        //     {
        //         return BadRequest();

        //     }    
        // }

        [HttpPost("createuserticket")]
        public async Task<IActionResult> createTicketUser([FromForm]TicketVM log)
        {
            // var Ticket = new TicketDetail()
            //     {
                    
            //         CategoryId = log.CategoryId.ToString(),
            //         UserId = log.UserId,
            //         DevId = null,
            //         DateIssued = DateTime.UtcNow,
            //         MessageContent =log.MessageContent,
            //         Status = "pending",
            //         CategoryName = log.CategoryName,
                    
            //     };
            var ticket=new TicketDetail();
            ticket.CategoryId=log.CategoryId;
            ticket.UserId=log.UserId;
            ticket.DateIssued=DateTime.Now;
            ticket.MessageContent=log.MessageContent;
            ticket.Status="pending";
            ticket.CategoryName=log.CategoryName;

            if(!log.Attachments.IsNullOrEmpty())
            {
                Console.WriteLine("ticket attachments");
                string links=await mailService.StoreAttachments(log.Attachments);
                //log.ticket.links=links;
            }

            // _context.Add(log.ticket);
            // await _context.SaveChangesAsync();
            // var tic=_context.TicketDetails.OrderBy(s=>s.TicketId).LastOrDefault();
            // MailRequest req= new MailRequest();
            // req.Body=tic.MessageContent;
            // req.Subject=tic.TicketId.ToString();
            try
            {
                // await mailService.SendEmailUser(req);
                // TicketResponse tr= new TicketResponse();
                // tr.ResponseMessage=req.Body;
                // tr.sender=tic.UserId.ToString();
                // tr.TicketId=req.Subject;
                // tr.date=DateTime.UtcNow;
                // _context.Add(tr);
                // await _context.SaveChangesAsync();


                return Ok(ticket);

            }
            catch (Exception ex)
            {

                Console.WriteLine("HellO\n\n"+ex);
                return BadRequest(ex);

            }    
        }


        //endpoint to return a list of tickets 
        [HttpGet("getalltickets")]
        public async Task<List<TicketDetail>> getTickets()
        {
            List<TicketDetail> td= _context.TicketDetails.ToList();
            return td;
        }

        [HttpGet("ticket")]
        public async Task<TicketDetail> getTickets(string? ticketID)
        {
            TicketDetail ticket=_context.TicketDetails.Select(s=>s).Where(s=>s.TicketId.ToString().Equals(ticketID)).FirstOrDefault();
            return ticket;
        }


        //:endpoint to get tickets of a dev
        [HttpGet("devTickets")]
        public async Task<List<TicketDetail>> getDevTickets(string? DevId)
        {     
            List<TicketDetail> td = _context.TicketDetails.Where(s => s.DevId.Equals(DevId)).ToList();
            return td;
        }

        //end point to return tickets of a user
        [HttpGet("userTickets")]
        public async Task<List<TicketDetail>> getUserTickets(string? userId)
        {
            List<TicketDetail> td = _context.TicketDetails.Where(s => s.UserId.ToString().Equals(userId)).ToList();
            return td;
        }


        //end point to return tickets within a date range and filter if a status is provided
        [HttpGet("filter")]
        public async Task<List<TicketDetail>> filter(string? startDate,string? endDate, string? status, string? userId, string? userRole)
        {
            DateTime today = DateTime.UtcNow;

            //default date range is 1 month
            DateTime defaultStartDate = today.AddMonths(-1).Date;
            DateTime defaultEndDate = today.Date;

            DateTime parsedStartDate = DateTime.Parse(startDate ?? defaultStartDate.Date.ToString("yyyy-MM-dd"));

            DateTime parsedEndDate = DateTime.Parse(endDate ?? defaultEndDate.Date.ToString("yyyy-MM-dd"));

            if(userId!=null)
            {
                if(userRole == "Staff"){

                    if(status != "All"){
                        List<TicketDetail> td = _context.TicketDetails.Where(s => s.DateIssued.Date >= parsedStartDate.Date && s.DateIssued.Date <= parsedEndDate.Date && s.DevId.ToString().Equals(userId) && s.Status.Equals(status)).ToList();
                        return td;
                    }else{
                        List<TicketDetail> td = _context.TicketDetails.Where(s => s.DateIssued.Date >= parsedStartDate.Date && s.DateIssued.Date <= parsedEndDate.Date && s.DevId.ToString().Equals(userId)).ToList();
                        return td;
                    }
                }else if (userRole == "Student"){
                    if(status != "All"){
                        List<TicketDetail> td = _context.TicketDetails.Where(s => s.DateIssued.Date >= parsedStartDate.Date && s.DateIssued.Date <= parsedEndDate.Date && s.UserId.ToString().Equals(userId) && s.Status.Equals(status)).ToList();
                        return td;
                    }else{
                        List<TicketDetail> td = _context.TicketDetails.Where(s => s.DateIssued.Date >= parsedStartDate.Date && s.DateIssued.Date <= parsedEndDate.Date && s.UserId.ToString().Equals(userId)).ToList();
                        return td;
                    }
                }else{
                    if(status != "All"){
                        List<TicketDetail> td = _context.TicketDetails.Where(s => s.DateIssued.Date >= parsedStartDate.Date && s.DateIssued.Date <= parsedEndDate.Date && s.Status.Equals(status)).ToList();
                        return td;
                    }else{
                        List<TicketDetail> td = _context.TicketDetails.Where(s => s.DateIssued.Date >= parsedStartDate.Date && s.DateIssued.Date <= parsedEndDate.Date).ToList();
                        return td;
                    }
                }
                
            }else{
                //for the admin side
                List<TicketDetail> td = _context.TicketDetails.Where(s => s.DateIssued.Date >= parsedStartDate.Date && s.DateIssued.Date <= parsedEndDate.Date).ToList();
                return td;
            }
            
        }

        [HttpGet("openTickets")]
        public async Task<List<TicketDetail>> getOpenTickets()
        {
            List<TicketDetail> td = _context.TicketDetails.Where(s => s.Status.Equals("open")).ToList();
            return td;
        }

        [HttpGet("closedTickets")]
        public async Task<List<TicketDetail>> getClosedTickets()
        {
            List<TicketDetail> td = _context.TicketDetails.Where(s => s.Status.Equals("closed")).ToList();
            return td;
        }

        [HttpGet("pendingTickets")]
        public async Task<List<TicketDetail>> getPendingTickets()
        {
            List<TicketDetail> td = _context.TicketDetails.Where(s => s.Status.Equals("pending")).ToList();
            return td;
        }

        [HttpGet("unopenedTickets")]
        public async Task<List<TicketDetail>> getUnopenedTickets()
        {
            List<TicketDetail> td = _context.TicketDetails.Where(s => s.Status.Equals("unopened")).ToList();
            return td;
        }

        //get all the ticket statuses
        [HttpGet("ticketStatuses")]
        public async Task<List<string>> getTicketStatuses()
        {
            List<string> td = _context.TicketDetails.Select(s=>s.Status).Distinct().ToList();
            return td;
        }


        //end point to edit a ticket
        [HttpPost("editTicket")]
        public async Task<IActionResult> editTicket([FromBody] TicketDetail ticket)
        {

            try
            {
                _context.Update(ticket);
                await _context.SaveChangesAsync();
                return Ok(ticket);
            }
            catch(Exception ex)
            {
                return BadRequest();
            }
            
            

        }

        //endpoint to close a ticket
        [HttpPost("closeTicket")]
        public async Task<IActionResult> closeTicket(string? ticketID,[FromBody] MailRequest request)
        {
            var ticket=_context.TicketDetails.Select(s=>s).Where(s=>s.TicketId.ToString().Equals(ticketID)).FirstOrDefault();
            Console.WriteLine("here");
            ticket.Status="closed";
            _context.Update(ticket);
            await _context.SaveChangesAsync();
            if(!ticket.UserId.HasValue)
            {
                var sender=_context.TicketResponses.Select(s=>s).Where(s=>s.TicketId.ToString().Equals(ticketID) && !s.sender.IsNullOrEmpty()).FirstOrDefault().sender;
                request.ToEmail=sender;
            }
            try
            {

                await mailService.SendEmailAdmin(request);
                TicketResponse tr= new TicketResponse();
                tr.ResponseMessage=request.Body;
                tr.TicketId=ticket.TicketId.ToString();
                tr.DevId=request.DevId;
                tr.date=DateTime.UtcNow;
                _context.Add(tr);
                await _context.SaveChangesAsync();
                
                return Ok(tr);
            }
            catch (Exception ex)
            {


                return BadRequest();

            }
            
        }
        

    }


}
