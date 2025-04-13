using Microsoft.AspNetCore.Html;
using MimeKit;
using MimeKit.Text;

namespace DotnetOrderService.Infrastructure.Email
{
    public class Email
    {
        private string _subject;
        private MailboxAddress _sender = null;
        private readonly List<MailboxAddress> _recipients = [];
        private readonly List<MailboxAddress> _cc = [];
        private readonly List<MailboxAddress> _bcc = [];
        private TextPart textPart = null;
        private readonly List<MimePart> attachments = [];

        public Email From(string name, string address)
        {
            _sender = new MailboxAddress(name, address);
            return this;
        }
        public Email To(string name, string address)
        {
            _recipients.Add(new MailboxAddress(name, address));
            return this;
        }

        public Email Cc(string name, string address)
        {
            _cc.Add(new MailboxAddress(name, address));
            return this;
        }

        public Email Bcc(string name, string address)
        {
            _bcc.Add(new MailboxAddress(name, address));
            return this;
        }

        public Email Subject(string subject)
        {
            _subject = subject;
            return this;
        }

        public Email PlainTextMessage(string text)
        {
            textPart = new TextPart(TextFormat.Plain)
            {
                Text = text
            };
            return this;
        }

        public Email HtmlMessage(HtmlString html)
        {
            textPart = new TextPart(TextFormat.Html)
            {
                Text = html.ToString()
            };
            return this;
        }

        public Email Attachment(FileStream stream, string fileName = null)
        {
            MimePart attachment = new()
            {
                Content = new MimeContent(stream),
                ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                ContentTransferEncoding = ContentEncoding.Base64,
                FileName = fileName ?? Path.GetFileName(stream.Name)
            };

            attachments.Add(attachment);
            return this;
        }

        public MimeMessage BuildMessage(EmailConfig emailConfig)
        {
            _sender ??= new MailboxAddress(emailConfig.SenderName, emailConfig.SenderAddress);
            
            MimeMessage message = new();
            message.From.Add(_sender);
            message.To.AddRange(_recipients);
            message.Cc.AddRange(_cc);
            message.Bcc.AddRange(_bcc);
            message.Subject = _subject;

            if (attachments.Count == 0)
            {
                message.Body = textPart;
            }
            else
            {
                Multipart multipart = new("mixed")
                {
                    textPart
                };

                foreach (MimePart attachment in attachments)
                {
                    multipart.Add(attachment);
                }

                message.Body = multipart;
            }

            return message;
        }
    }
}