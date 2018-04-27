using System.Reflection;
using System.Security;

#if !NETSTANDARD2_0
[assembly: AssemblyProduct("Common Logging Framework NLog 4.4.12 Adapter")]
#endif
[assembly: SecurityTransparent]

#if NET_4_0
[assembly: SecurityRules(SecurityRuleSet.Level1)]
#endif