using System;
using System.Threading;
using SmtpServer;

namespace MailClient.Tests
{
    public class TestMailServer
    {
        public FakeMessageStore MessageStore { get; }

        /// <summary>
        /// The cancellation token source for the test.
        /// </summary>
        public CancellationTokenSource CancellationTokenSource { get; }


        public TestMailServer(FakeMessageStore store, CancellationTokenSource tokenSource)
        {
            MessageStore = store;
            CancellationTokenSource = tokenSource;
        }

        public SmtpServerDisposable Start()
        {
            var options = new SmtpServerOptionsBuilder()
                .ServerName("localhost")
                .Port(25, 587)
                .MessageStore(MessageStore)
                .Build();
            var server = new SmtpServer.SmtpServer(options);
            var smtpServerTask = server.StartAsync(CancellationTokenSource.Token);

            return new SmtpServerDisposable(server, () =>
            {
                CancellationTokenSource.Cancel();

                try
                {
                    smtpServerTask.Wait();
                }
                catch (AggregateException e)
                {
                    e.Handle(exception => exception is OperationCanceledException);
                }
            });
        }
    }
}