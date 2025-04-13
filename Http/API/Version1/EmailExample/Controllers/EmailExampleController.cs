using DotnetOrderService.Infrastructure.Email;
using DotnetOrderService.Infrastructure.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;

namespace DotnetOrderService.Http.API.Version1.EmailExample.Controllers
{
    // This controller is used for testing only, please delete when using the boilerplate
    [Route("api/v1/emails")]
    [ApiController]
    [AllowAnonymous]
    public class EmailExampleController(EmailService emailService, ILogger<EmailExampleController> logger) : ControllerBase
    {
        [HttpPost("test-send")]
        public async Task<ApiResponseData<object>> TestSend()
        {

            // Simple email
            Email simpleEmail = new Email()
                .Subject("Test Email Simple")
                .To("John Doe", "john@example.com")
                .PlainTextMessage("This is a test email message");


            await emailService.SendEmailAsync(simpleEmail);

            // More complex email with attachments

            FileStream attachment1 = System.IO.File.OpenRead("appsettings.example.json");
            FileStream attachment2 = System.IO.File.OpenRead("docker-compose.yml");

            Email plainTextEmail = new Email()
                .Subject("Test Email Plain Text")
                .To("John Doe", "john@example.com")
                .Cc("Jane Doe", "jane@example.com")
                .PlainTextMessage("This is a test email message")
                .Attachment(attachment1)
                .Attachment(attachment2);

            Email htmlEmail = new Email()
                .Subject("Test Email HTML")
                .To("John Doe", "john@example.com")
                .Cc("Jane Doe", "jane@example.com")
                .HtmlMessage(new HtmlString("<h1>This is a test email message</h1>"))
                .Attachment(attachment1)
                .Attachment(attachment2);

            // You can send multiple emails at once

            await emailService.SendMultipleEmailAsync([plainTextEmail, htmlEmail], (sender, args) =>
            {
                // This is callback function when email sent
                logger.LogInformation("Message sent: {MessageId}", args.Message.MessageId);
            });

            return new ApiResponseData<object>(System.Net.HttpStatusCode.OK, new { message = "Email sent successfully" });
        }
    }
}

