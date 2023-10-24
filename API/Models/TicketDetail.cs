using System;
using System.Collections.Generic;

namespace api.Models;

public partial class TicketDetail
{
    public int TicketId { get; set; }

    public string CategoryId { get; set; }

    public string DevId { get; set; }

    public DateTime DateIssued { get; set; }

    public string MessageContent { get; set; }

    public string? Links {get;set;}
    public string? Notes {get;set;}
    public int? Priority {get;set;}

    public string? Status { get; set; }

    public string CategoryName { get; set; }

    public int? UserId { get; set; }
}
