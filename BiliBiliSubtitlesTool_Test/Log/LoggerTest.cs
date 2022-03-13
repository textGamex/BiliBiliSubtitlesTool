using System;
using NUnit.Framework;
using BiliBiliSubtitlesTool.Log;

namespace BiliBiliSubtitlesTool_Test.Log
{
    internal class LoggerTest
    {
        [Test]
        public void SetLoggerExectionTest()
        {
            Assert.Throws<ArgumentNullException>(() => Logger.SetLogger(null));
        }
    }
}
