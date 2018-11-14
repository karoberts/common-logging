#if NET_3_0 || NET_4_0

using Common.Logging;

namespace Common
{
    class AnotherClass
    {
        public void CanCompile()
        {
#pragma warning disable CS0618 // Type or member is obsolete
            ILog log = LogManager.GetCurrentClassLogger();
#pragma warning restore CS0618 // Type or member is obsolete
            log.Trace(m=> m("test {0}", "test"));
        }
    }
}

#endif