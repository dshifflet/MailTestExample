using System;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MimeKit;
using SmtpServer.Mail;

namespace MailClient.Tests
{
    [TestClass]
    public class MailClientTests
    {
        [TestMethod]
        public void CanSendMailWithAttachments()
        {
            var store = new FakeMessageStore();
            var mailServer = new TestMailServer(store, new CancellationTokenSource());
            using (var server = mailServer.Start())
            {
                var client = new MailClient("localhost", 25, "test", "test");

                var subject = "This is some text in the subject";
                var body = "This some text in the body";
                var attachmentBody = "Sample File";

                client.SendMail("test@test.com",
                    "test@test.com",
                    subject,
                    body,
                    new[] {new Attachment(new MemoryStream(Encoding.UTF8.GetBytes(attachmentBody)), "sample.txt")});

                Assert.IsTrue(store.Messages.Any());

                //Need MimeKit to decode the message so we can work with attachments.
                var message = MimeMessage.Load(((ITextMessage) store.Messages.First().Message).Content);
                Assert.IsTrue(message.Attachments.Count()==1);
                foreach (var testAttachment in message.Attachments)
                {
                    if (!(testAttachment is MimePart part)) continue;

                    using (var ms = new MemoryStream())
                    {
                        part.Content.DecodeTo(ms);
                        ms.Seek(0, SeekOrigin.Begin);
                        using (var sr = new StreamReader(ms))
                        {
                            Assert.AreEqual(sr.ReadToEnd(), attachmentBody);
                        }
                    }
                }
                Assert.AreEqual(message.TextBody, body);
                Assert.AreEqual(message.Subject, subject);
            }
        }
    }
}
