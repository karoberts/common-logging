using System;
using System.Collections.Generic;
using System.Text;
using Common.Logging.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Common.Logging.MicrosoftLogging
{
    /// <summary>
    /// Concrete subclass of ILoggerFactoryAdapter specific to Microsoft Logging.
    /// </summary>
    /// <example>
    /// <code>
    /// LogManager.Adapter = new MSLoggerFactoryAdapter(iLogger);
    /// </code>
    /// OR
    /// <code>
    /// LogManager.Adapter = new MSLoggerFactoryAdapter(serviceProvider);
    /// </code>
    /// </example>
    public class MSLoggerFactoryAdapter : ILoggerFactoryAdapter
    {
        private readonly ILogger _logger;
        private readonly MSLogger _msLogger;

        /// <summary>
        /// Not Supported
        /// </summary>
        /// <param name="properties"></param>
        public MSLoggerFactoryAdapter(NameValueCollection properties)
        {
            throw new NotSupportedException("microsoft logging cannot be constructed from common.logging config");
        }

        public MSLoggerFactoryAdapter(ILogger logger)
        {
            _logger = logger;
            _msLogger = new MSLogger(_logger);
        }

        public MSLoggerFactoryAdapter(IServiceProvider service)
        {
            _logger = service.GetRequiredService<ILogger>();
            _msLogger = new MSLogger(_logger);
        }

        public ILog GetLogger(Type type) => _msLogger;

        public ILog GetLogger(string key) => _msLogger;
    }
}