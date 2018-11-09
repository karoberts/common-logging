
using System.Runtime.CompilerServices;

using Common.Logging;

[assembly: System.Reflection.AssemblyProduct("Common Logging Framework")]

#if !NETSTANDARD20
[assembly: System.Security.SecurityTransparent]
#endif

[assembly: TypeForwardedTo(typeof(FormatMessageHandler))]
[assembly: TypeForwardedTo(typeof(IConfigurationReader))]
[assembly: TypeForwardedTo(typeof(ILog))]
[assembly: TypeForwardedTo(typeof(ILoggerFactoryAdapter))]
[assembly: TypeForwardedTo(typeof(ILogManager))]
[assembly: TypeForwardedTo(typeof(IVariablesContext))]
[assembly: TypeForwardedTo(typeof(LogLevel))]