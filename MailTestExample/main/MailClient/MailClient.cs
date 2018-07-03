using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;

namespace MailClient
{
    public class MailClient
    {
        private readonly string _smtpHost;
        private readonly int _smtpPort;
        private readonly string _smtpUserName;
        private readonly string _smtpPassword;

        public MailClient(string smtpHost, int smtpPort, string smtpUserName, string smtpPassword)
        {
            if (smtpHost == null) throw new ArgumentException("smtpHost");
            if (smtpUserName == null) throw new ArgumentException("smtpUserName");
            if (smtpPassword == null) throw new ArgumentException("smtpPassword");

            _smtpHost = smtpHost;
            _smtpPort = smtpPort;
            _smtpUserName = smtpUserName;
            _smtpPassword = smtpPassword;
        }

        public void SendMail(MailMessage message)
        {
            SendMail(new[] { message });
        }

        public void SendMail(IEnumerable<MailMessage> messages)
        {
            using (var client = new SmtpClient())
            {
                client.UseDefaultCredentials = true;
                client.Host = _smtpHost;
                client.Port = _smtpPort;
                client.Credentials =
                    new NetworkCredential(_smtpUserName, _smtpPassword);
                foreach (var message in messages)
                {
                    client.Send(message);
                }
            }
        }

        public void SendMail(string from, string recipient, string subject, string body,
            IEnumerable<Attachment> attachments)
        {
            var message = new MailMessage(from, recipient, subject, body);
            if (attachments != null)
            {
                foreach (var a in attachments)
                {
                    message.Attachments.Add(a);
                }
            }
            SendMail(new[] { message });
        }
    }
}
