using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SmtpServer;
using SmtpServer.Protocol;
using SmtpServer.Storage;

namespace MailClient.Tests
{
    public class FakeMessageStore : MessageStore
    {
        public FakeMessageStore()
        {
            Messages = new List<IMessageTransaction>();
        }

        public override Task<SmtpResponse> SaveAsync(ISessionContext context, IMessageTransaction transaction, CancellationToken cancellationToken)
        {
            Messages.Add(transaction);

            return Task.FromResult(SmtpResponse.Ok);
        }

        public List<IMessageTransaction> Messages { get; }
    }
}