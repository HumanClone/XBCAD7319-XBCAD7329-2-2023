using System;
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
        private readonly XbcadDb2Context _context;
        private readonly IMailService mailService;

        public TicketController(XbcadDb2Context context,IMailService mailService)
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

        //TODO:Test adding response
        [HttpPost("createuserticket")]
        public async Task<IActionResult> createTicketUser([FromForm]TicketVM log)
        {
           
            Console.WriteLine("here is the category id:"+log.CategoryId);
            Console.WriteLine("here is the USer id "+log.UserId);
            Console.WriteLine("here is the MEssage "+log.MessageContent);
            Console.WriteLine("here is the Category Name :"+log.CategoryName);

            var ticket=new TicketDetail();

            ticket.CategoryId=log.CategoryId;
            ticket.UserId=log.UserId;
            ticket.DateIssued=DateTime.Now;
            ticket.MessageContent=log.MessageContent;
            ticket.Status="pending";
            ticket.CategoryName=log.CategoryName;
            ticket.Priority = (int)Priority.Low;

            if(!log.Attachments.IsNullOrEmpty())
            {
                Console.WriteLine("ticket attachments");
                string links=await mailService.StoreAttachments(log.Attachments);
                ticket.Links=links;
            }

            _context.Add(ticket);
            await _context.SaveChangesAsync();
            var tic=_context.TicketDetails.OrderBy(s=>s.TicketId).LastOrDefault();
            MailRequest req= new MailRequest();
            req.Body=tic.MessageContent;
            req.Subject=tic.TicketId.ToString();
            try
            {
                await mailService.SendEmailUser(req);
                TicketResponse tr= new TicketResponse();
                tr.ResponseMessage=req.Body;
                tr.Sender=tic.UserId.ToString();
                tr.TicketId=req.Subject;
                tr.Date=DateTime.Now;

                _context.Add(tr);
                await _context.SaveChangesAsync();



                return Ok(ticket);

            }
            catch (Exception ex)
            {

                Console.WriteLine("HellO\n\n"+ex);
                return BadRequest();

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
            List<TicketDetail> td = _context.TicketDetails.Where(s => s.UserId.ToString().Equals(userId)).OrderByDescending(s => s.DateIssued).ToList();
            return td;
        }


        //end point to return tickets within a date range and filter if a status is provided
        [HttpGet("filter")]
        public async Task<List<TicketDetail>> filter(string? startDate,string? endDate, string? status, string? category,string? priority,string? userId, string? userRole)
        {  

            var query = _context.TicketDetails.AsQueryable();

            if(startDate != null && endDate != null){
                DateTime parsedStartDate = DateTime.Parse(startDate);
                DateTime parsedEndDate = DateTime.Parse(endDate);
                query = query.Where(s => s.DateIssued.Date >= parsedStartDate.Date && s.DateIssued.Date <= parsedEndDate.Date);
            }else if(startDate == null && endDate != null){
                DateTime parsedEndDate = DateTime.Parse(endDate);
                query = query.Where(s => s.DateIssued.Date <= parsedEndDate.Date);
            }else if(endDate == null && startDate != null){
                DateTime parsedStartDate = DateTime.Parse(startDate);
                query = query.Where(s => s.DateIssued.Date >= parsedStartDate.Date);
            }
            
            if (userId != null)
            {
                if (userRole == "Staff")
                {
                    query = query.Where(s => s.DevId == userId);
                }
                else if (userRole == "Student")
                {
                    query = query.Where(s => s.UserId.ToString().Equals(userId));
                }
            }

             Console.WriteLine(query);
            
            if (status != "All")
            {
                query = query.Where(s => s.Status.Equals(status));
            }

            if (category != "All")
            {
                query = query.Where(s => s.CategoryName.Equals(category));
            }

            if (priority != "All")
            {
                Priority parsedPriority = (Priority)Enum.Parse(typeof(Priority), priority);
                int priorityValue = (int)parsedPriority;
                query = query.Where(s => s.Priority == priorityValue);
            }

            
            List<TicketDetail> td = query.OrderByDescending(s => s.DateIssued).ToList();
            return td;
            
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

        //get the number of tickets for each status that is not null, return a dictionary where the key is the status and the value is the number of tickets
        [HttpGet("ticketStatusCount")]
        public async Task<Dictionary<string,int>> getTicketStatusCount()
        {
            Dictionary<string,int> td = _context.TicketDetails.Where(s=>s.Status!=null).GroupBy(s=>s.Status.ToUpper()).ToDictionary(s=>s.Key,s=>s.Count());
            return td;
        }

        [HttpGet("ticketPriorityCount")]
        public async Task<Dictionary<string,int>> getTicketPriorityCount()
        {
            Dictionary<string, int> priorityCounts = new Dictionary<string, int>();
            
            var ticketDetails = await getTickets();
                
            foreach (var ticket in ticketDetails)
            {
                string priorityName = GetPriorityName(ticket.Priority);
                if (priorityCounts.ContainsKey(priorityName))
                {
                    priorityCounts[priorityName]++;
                }
                else
                {
                    priorityCounts[priorityName] = 1;
                }
            }
            
            return priorityCounts;           
        }

        [HttpGet("getPriorityName")]
        public string GetPriorityName(int? priority)
        {
            string priorityName = Enum.GetName(typeof(Priority), priority);
            return priorityName.ToUpper().Replace("_", " ");
        }

        [HttpGet("getAllPriorityNames")]
        public IEnumerable<string> GetAllPriorityNames()
        {
            return Enum.GetNames(typeof(Priority));
        }

        [HttpGet]
        [Route("countByStatusAndDate")]
        public async Task<TicketData> GetTicketCountByStatusAndDate()
        {
            var status = Request.Query["status"];
            var date = DateTime.Parse(Request.Query["date"]);
            
            var tickets = await getTickets();
        
            // Filter the tickets based on the specified status and date
            int count = tickets.Count(ticket => ticket.Status != null && ticket.Status.ToUpper() == status && ticket.DateIssued.Date == date.Date);
            var data = new TicketData
            {
                Date = date,
                Count = count
            };
        
            return data;
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
        [HttpGet("closeTicket")]
        public async Task<IActionResult> closeTicket(string? ticketID)
        {
            var ticket=_context.TicketDetails.Select(s=>s).Where(s=>s.TicketId.ToString().Equals(ticketID)).FirstOrDefault();
            MailRequest rq=new MailRequest();
            Console.WriteLine("here");
            ticket.Status="closed";
            _context.Update(ticket);
            await _context.SaveChangesAsync();
            if(!ticket.UserId.HasValue)
            {
                var id =int.Parse(ticketID);
                var sender=_context.TicketResponses.Select(s=>s).Where(s=>s.TicketId==ticketID && s.Sender!=null).FirstOrDefault().Sender;
                rq.ToEmail=sender;
            }
            else
            {
                rq.ToEmail=_context.UserInfos.Where(user=>user.UserId==ticket.UserId).Select(s=>s.Email).First();
            }
            try
            {
                
                rq.Subject="Re:"+ticketID;
                rq.Body="Your ticket has been close";
                await mailService.SendEmailAdmin(rq);
                TicketResponse tr= new TicketResponse();
                tr.Date=DateTime.Now;
                tr.ResponseMessage=rq.Body;
                tr.TicketId=ticket.TicketId.ToString();
                tr.DevId=ticket.DevId;
                tr.Date=DateTime.UtcNow;
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
