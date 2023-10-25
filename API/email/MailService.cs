using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using Azure.Storage.Blobs;

//https://codewithmukesh.com/blog/send-emails-with-aspnet-core/
namespace api.email;

public class MailService :IMailService
{
    private readonly adminmail _adminmail;
    private readonly usermail _usermail;
    private readonly IConfiguration _configuration;
    private readonly BlobContainerClient _containerClient;

    public MailService(IOptions<adminmail> mailSettings1,IOptions<usermail> mailSettings2 ,IConfiguration configuration)
    {
        _adminmail = mailSettings1.Value;
        _usermail = mailSettings2.Value;
        _configuration = configuration;
            BlobServiceClient blobServiceClient = new BlobServiceClient(_configuration.GetConnectionString("BlobString"));
            _containerClient = blobServiceClient.GetBlobContainerClient("attachments");
    }

    public async Task SendEmailUser(MailRequest mailRequest)
    {
        var email = new MimeMessage();
        email.Sender = MailboxAddress.Parse(_usermail.Mail);
        Console.WriteLine(_usermail.Mail);
        email.To.Add(MailboxAddress.Parse(_adminmail.Mail));
        Console.WriteLine(_adminmail.Mail);
        email.Subject = mailRequest.Subject;
        var builder = new BodyBuilder();
        if (mailRequest.Attachments != null)
        {
            byte[] fileBytes;
            foreach (var file in mailRequest.Attachments)
            {
                if (file.Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        file.CopyTo(ms);
                        fileBytes = ms.ToArray();
                    }
                    builder.Attachments.Add(file.FileName, fileBytes, ContentType.Parse(file.ContentType));
                }
            }
        }
        builder.HtmlBody = mailRequest.Body;
        email.Body = builder.ToMessageBody();
        using var smtp = new SmtpClient();
        smtp.Connect(_usermail.Host, _usermail.Port, SecureSocketOptions.StartTls);
        smtp.Authenticate(_usermail.Mail, _usermail.Password);
        await smtp.SendAsync(email);
        smtp.Disconnect(true);

        
    }


    public async Task SendEmailAdmin(MailRequest mailRequest)
    {
        var email = new MimeMessage();
        email.Sender = MailboxAddress.Parse(_adminmail.Mail);
        if (String.IsNullOrEmpty(mailRequest.ToEmail))
        {
            email.To.Add(MailboxAddress.Parse(_usermail.Mail));
        }
        else
        {
            email.To.Add(MailboxAddress.Parse(mailRequest.ToEmail));
        }
        
        email.Subject = mailRequest.Subject;
        var builder = new BodyBuilder();
        if (mailRequest.Attachments != null)
        {
            byte[] fileBytes;
            foreach (var file in mailRequest.Attachments)
            {
                if (file.Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        file.CopyTo(ms);
                        fileBytes = ms.ToArray();
                    }
                    builder.Attachments.Add(file.FileName, fileBytes, ContentType.Parse(file.ContentType));
                }
            }
        }
        builder.HtmlBody = mailRequest.Body;
        email.Body = builder.ToMessageBody();
        using var smtp = new SmtpClient();
        smtp.Connect(_adminmail.Host, _adminmail.Port, SecureSocketOptions.StartTls);
        smtp.Authenticate(_adminmail.Mail, _adminmail.Password);
        await smtp.SendAsync(email);
        smtp.Disconnect(true);

        
    }


    public async Task SendEmailAsync(MailRequest mailRequest)
    {
        var email = new MimeMessage();
        email.Sender = MailboxAddress.Parse(_adminmail.Mail);
        email.To.Add(MailboxAddress.Parse(mailRequest.ToEmail));
        email.Subject = mailRequest.Subject;
        var builder = new BodyBuilder();
        if (mailRequest.Attachments != null)
        {
            byte[] fileBytes;
            foreach (var file in mailRequest.Attachments)
            {
                if (file.Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        file.CopyTo(ms);
                        fileBytes = ms.ToArray();
                    }
                    builder.Attachments.Add(file.FileName, fileBytes, ContentType.Parse(file.ContentType));
                }
            }
        }
        builder.HtmlBody = mailRequest.Body;
        email.Body = builder.ToMessageBody();
        using var smtp = new SmtpClient();
        smtp.Connect(_adminmail.Host, _adminmail.Port, SecureSocketOptions.StartTls);
        smtp.Authenticate(_adminmail.Mail, _adminmail.Password);
        await smtp.SendAsync(email);
        smtp.Disconnect(true);


    }


    public async Task<string> StoreAttachments(List<IFormFile> files)
    {
        string links = "";
        
        foreach (var attachment in files)
        {
            using (var stream = attachment.OpenReadStream())
            {
                string blobName = $"{Guid.NewGuid()}_{attachment.FileName}";
                BlobClient blobClient = _containerClient.GetBlobClient(blobName);

                await blobClient.UploadAsync(stream, true);

                Uri blobUri = blobClient.Uri;
                links += blobUri + ";"; // Added newline for better formatting
                Console.WriteLine($"Attachment saved: {blobName}");
                Console.WriteLine($"Link to attachment: {blobUri}");
            }
        }
        links=links.Substring(0,links.Length-1);
        return links;
    }

    
}