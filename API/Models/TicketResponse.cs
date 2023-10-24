using System;
using System.Collections.Generic;

namespace api.Models;

public partial class TicketResponse
{
    public int ResponseId { get; set; }

    public string TicketId { get; set; }

    public string? DevId { get; set; }

    public string ResponseMessage { get; set; }
    public string? Links {get;set;}

    public DateTime? Date { get; set; }

    public string? Sender { get; set; }
}
