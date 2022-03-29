using System;
using BiliBiliSubtitlesTool.Lyric;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Threading.Tasks;

namespace BiliBiliSubtitlesTool_Test.Lyric
{
    [TestFixture]
    public class Lyric
    {
        [Test]
        public void NumberToTimeTagException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new TimeTag(60, 59, 999));
            Assert.Throws<ArgumentOutOfRangeException>(() => new TimeTag(60, 60, 999));
            Assert.Throws<ArgumentOutOfRangeException>(() => new TimeTag(60, 60, 1000));
        }

        [Test]
        public void StringToTimeTagExcepiton()
        {
            Assert.Throws<ArgumentNullException>(() => new TimeTag(null));
            Assert.Throws<FormatException>(() => new TimeTag(""));
            Assert.Throws<FormatException>(() => new TimeTag("[11.22.333]"));
            Assert.Throws<FormatException>(() => new TimeTag("[11.22:333]"));
            Assert.Throws<FormatException>(() => new TimeTag("[11:22:333]"));
            Assert.Throws<FormatException>(() => new TimeTag("[1.2.333]"));
            Assert.Throws<FormatException>(() => new TimeTag("[15.23.3]"));
        }

        [Test]
        public void NumberStringParseException()
        {
            Assert.Throws<FormatException>(() => new TimeTag("[11:22.1000]"));
            Assert.Throws<FormatException>(() => new TimeTag("[11:60.1000]"));
            Assert.Throws<FormatException>(() => new TimeTag("[59:60.999]"));
            Assert.Throws<FormatException>(() => new TimeTag("[60:59.999]"));
            Assert.Throws<FormatException>(() => new TimeTag("[60:60.1000]"));
        }

        [Test]
        public void NotHasException()
        {
            Assert.DoesNotThrow(() => new TimeTag("[1:2.3]"));
            Assert.DoesNotThrow(() => new TimeTag("[11:2.3]"));
            Assert.DoesNotThrow(() => new TimeTag("[11:22.3]"));
            Assert.DoesNotThrow(() => new TimeTag("[11:22.33]"));
            Assert.DoesNotThrow(() => new TimeTag("[11:22.333]"));
        }

        [Test]
        public void GetTiemTag()
        {
            TimeTag timeTag_1 = (TimeTag)"[11:22.333]";
            TimeTag timeTag_2 = (TimeTag)"[1:2.33]";

            Assert.AreEqual(11, timeTag_1.Minute);
            Assert.AreEqual(22, timeTag_1.Second);
            Assert.AreEqual(333, timeTag_1.Millisecond);
            Assert.AreEqual(1, timeTag_2.Minute);
            Assert.AreEqual(2, timeTag_2.Second);
            Assert.AreEqual(330, timeTag_2.Millisecond);
        }
    }
}
