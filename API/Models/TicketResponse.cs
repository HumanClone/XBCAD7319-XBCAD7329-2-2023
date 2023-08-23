using System;
using System.Collections.Generic;

namespace api.Models;

public partial class TicketResponse
{
    public int ResponseId { get; set; }

    public string TicketId { get; set; }

    public string DevId { get; set; }

    public string Email { get; set; }

    public string ResponseMessage { get; set; }
}
