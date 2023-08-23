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

    public string Status { get; set; }
}
