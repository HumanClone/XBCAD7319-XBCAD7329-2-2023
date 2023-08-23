namespace api.email;

public class MailReceive
{
    public string FromEmail { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }

    public DateTime ReceivedDate {get;set;}
    //public List<IFormFile>? Attachments { get; set; }
    
}