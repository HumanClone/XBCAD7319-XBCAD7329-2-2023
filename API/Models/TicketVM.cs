namespace api.Models
{
    public class TicketVM
    {
        public string? CategoryId {get; set;}
        
        public string? DevId {get; set;}

        public int? UserId {get; set;}
        public string? CategoryName {get; set;}
        public string? MessageContent {get; set;}
        public string? Status {get; set;} = "Pending";
        public List<IFormFile>? Attachments { get; set; }
    }
}