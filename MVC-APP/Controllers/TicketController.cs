
using System.Net;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MVCAPP.Data;
using MVCAPP.Models;

using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace mvc_app.Controllers;

public class TicketController : Controller
{
    private readonly ApplicationDbContext _context;

    public TicketController(ApplicationDbContext context)
    {
       this._context = context;
    }

    [HttpGet]
        public IActionResult Create(string categoryName)
        {
           
            ViewBag.SelectedCategoryName = categoryName;
            return View();
        }  
    
     [HttpPost]
     public IActionResult Create(TicketDetail ticketDetail, string categoryName) 
        {
            try
            {
                string selectedCategory = categoryName;
              //string selectedCategory = HttpContext.Session.GetString("SelectedCategory");
           //HttpContext.Session

                if (ModelState.IsValid)
                {
                    var Ticket = new TicketDetail()
                    {

                        
                        TicketId = ticketDetail.TicketId,
                        CategoryId = ticketDetail.CategoryId,
                        UserId = ticketDetail.UserId,
                        DevId = ticketDetail.DevId,
                        DateIssued = ticketDetail.DateIssued,
                        MessageContent = ticketDetail.MessageContent,
                        Status = ticketDetail.Status,
                        CategoryName = selectedCategory 
                        
                    };//send this object to api
                        
                
                    _context.TicketDetails.Add(Ticket);
                    _context.SaveChanges();
                  
                    Console.WriteLine("Added to database");
                    return RedirectToAction("ViewTicket");
                }
                else
                {
                    foreach (var modelState in ModelState.Values)
                        {
                            foreach (var error in modelState.Errors)
                            {
                                Console.WriteLine($"Model Error: {error.ErrorMessage}");
                            }
                        }

                    return View();
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();

            }
        }


        [HttpGet]
        public IActionResult ViewTicket()
        {
            var tickets = _context.TicketDetails.ToList();
            List<TicketDetail> ticketList = new List<TicketDetail>();

            foreach (var ticket in tickets)
            {
                var ticketDetail = new TicketDetail
                {
                    TicketId = ticket.TicketId,
                    CategoryName = ticket.CategoryName,
                    MessageContent = ticket.MessageContent,
                    DateIssued = ticket.DateIssued,
                    Status = ticket.Status
                };
              
                ticketList.Add(ticketDetail);
            }

            return View(ticketList);
        }

}
