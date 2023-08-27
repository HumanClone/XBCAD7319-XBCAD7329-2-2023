using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MVCAPP.Data;
using MVCAPP.Models;

namespace mvc_app.Controllers;

public class TicketController : Controller
{
    private readonly ApplicationDbContext _context;

    public TicketController(ApplicationDbContext context)
    {
       this._context = context;
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }  
    
     [HttpPost]
     public IActionResult Create(TicketDetail ticketDetail) 
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var Ticket = new TicketDetail()
                    {
                        TicketId = ticketDetail.TicketId,
                        CategoryId = ticketDetail.CategoryId,
                        DevId = ticketDetail.DevId,
                        DateIssued = ticketDetail.DateIssued,
                        MessageContent = ticketDetail.MessageContent,
                        Status = ticketDetail.Status,
                    };

                    _context.TicketDetails.Add(Ticket);
                    _context.SaveChanges();
                    TempData["successMessage"] = "Ticket Added";
                    Console.WriteLine("Added to database");
                    return RedirectToAction("Create");
                }
                else
                {
                    TempData["errorMessage"] = "Ticket data not valid";
                    return View();
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();

            }
        }
}