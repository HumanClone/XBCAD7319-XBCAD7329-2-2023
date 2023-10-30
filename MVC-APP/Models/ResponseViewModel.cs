namespace MVCAPP.Models
{

    public class ResponseViewModel
    {
        public TicketDetail Ticket { get; set; }
        public List<TicketResponse> Responses { get; set; }
        public string? Priority { get; set; }
        public List<string>? Links { get; set; }
        //public string Sender { get; set; }
    }

}