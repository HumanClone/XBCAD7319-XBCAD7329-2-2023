namespace MVCAPP.Models;

public class MailRequest
{
    public string ToEmail { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
    public List<IFormFile>? Attachments { get; set; }

    public string? DevId {get;set;}
    public string? UserId {get;set;}
}