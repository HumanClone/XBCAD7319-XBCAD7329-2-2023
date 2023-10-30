namespace MVCAPP.Templates
{
    public class Template
    {
        public string Name { get; set; }
        public string Content { get; set; }
    }

    public class TemplateService
    {
        private List<Template> templates;

        public TemplateService()
        {
            templates = new List<Template>
        {
            new Template
            {
                Name = "Request for more information",
                Content = @"Hello [Student's Name],

This is [Name], and I'm here to help you with your issue. In order to assist you effectively, could you please provide me with the following information:

1. A detailed description of the problem.
2. Any error messages or codes you encountered.
3. The steps you've already taken to resolve the issue.

Thank you for your cooperation, and we'll do our best to resolve this issue promptly.

Best regards,
[Name]
[Title]"
            },
            new Template
            {
                Name = "Following up",
                Content = @"Hello [Student's Name],

I hope this message finds you well. We have been working on resolving your reported issue. I wanted to check in and ask if the problem has been fixed on your end. If it has, please let us know, and if not, provide any additional details or feedback on the progress.

Your feedback is valuable, and we're committed to ensuring your satisfaction.

Best regards,
[Name]
[Title]"
            },
            new Template
            {
                Name = "Escalating the Ticket",
                Content = @"Hello [Student's Name],

I hope you're well. We have reviewed your issue and have decided to escalate its priority due to its critical nature. Our team will be working diligently to resolve this issue as quickly as possible.

We understand the urgency, and your satisfaction is our top priority. We'll keep you updated on our progress.

Best regards,
[Name]
[Title]"
            }
        };
        }

        public string GetTemplateContent(string templateName)
        {
            Template template = templates.FirstOrDefault(t => t.Name == templateName);
            return template?.Content;
        }

        public List<string> GetTemplateNames()
        {
            return templates.Select(t => t.Name).ToList();
        }
    }
}