using System;

namespace MailClient.Tests
{
    public sealed class SmtpServerDisposable : IDisposable
    {
        readonly Action _delegate;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="server">The SMTP server instance.</param>
        /// <param name="delegate">The delegate to execute upon disposal.</param>
        public SmtpServerDisposable(SmtpServer.SmtpServer server, Action @delegate)
        {
            Server = server;

            _delegate = @delegate;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _delegate();
        }

        /// <summary>
        /// The SMTP server instance.
        /// </summary>
        public SmtpServer.SmtpServer Server { get; }
    }
}