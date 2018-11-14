#if NET_3_0 || NET_4_0

using System;
using System.Globalization;
using Common.Logging;
using NUnit.Framework;

namespace Common
{
    /// <summary>
	/// Summary description for Class1.
	/// </summary>
	[TestFixture]
	public class Class1
	{
        [Test]
		public void CanCompile()
		{
#pragma warning disable CS0618 // Type or member is obsolete
            ILog log = LogManager.GetCurrentClassLogger();
#pragma warning restore CS0618 // Type or member is obsolete
            log.Trace(CultureInfo.InvariantCulture,  m => m("test {0}", "test"), new Exception());
		}
	}
}

#endif