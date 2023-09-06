[Authorize(Roles = "Staff")]
public IActionResult StaffViewTickets()
{
    var tickets = _context.TicketDetails.ToList(); // Retrieve all tickets
    return View(tickets);
}
