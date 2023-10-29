namespace MVCAPP.Models
{

    public class ResponseViewModel
    {
        public TicketDetail Ticket { get; set; }
        public List<TicketResponse> Responses { get; set; }
        public string Priority { get; set; }
    }

}