using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

//https://codewithmukesh.com/blog/send-emails-with-aspnet-core/
namespace api.email;

public class MailService :IMailService
{
    private readonly MailSettings _mailSettings1;
    private readonly MailSettings _mailSettings2;

    public MailService(IOptions<MailSettings> mailSettings1, IOptions<MailSettings> mailSettings2)
    {
        _mailSettings1 = mailSettings1.Value;//user
        _mailSettings2 = mailSettings2.Value;//admin
    }
    public async Task SendEmailUser(MailRequest mailRequest)
    {
        var email = new MimeMessage();
        email.Sender = MailboxAddress.Parse(_mailSettings1.Mail);
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
        smtp.Connect(_mailSettings1.Host, _mailSettings1.Port, SecureSocketOptions.StartTls);
        smtp.Authenticate(_mailSettings1.Mail, _mailSettings1.Password);
        await smtp.SendAsync(email);
        smtp.Disconnect(true);

        
    }


    public async Task SendEmailAdmin(MailRequest mailRequest)
    {
        var email = new MimeMessage();
        email.Sender = MailboxAddress.Parse(_mailSettings2.Mail);
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
        smtp.Connect(_mailSettings2.Host, _mailSettings2.Port, SecureSocketOptions.StartTls);
        smtp.Authenticate(_mailSettings2.Mail, _mailSettings2.Password);
        await smtp.SendAsync(email);
        smtp.Disconnect(true);

        
    }
}