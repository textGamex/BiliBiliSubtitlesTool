using System;
using BiliBiliSubtitlesTool.Lyric;
using NUnit.Framework;

namespace BiliBiliSubtitlesTool_Test.Lyric
{
    [TestFixture]
    public class Lyric
    {
        private TimeTag small = new TimeTag(1, 0, 0);
        private TimeTag equalSmall = new TimeTag(1, 0, 0);
        private TimeTag big = new TimeTag(2, 0, 0);

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
        public void GetTiemTagOfNumber()
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

        [Test]
        public void PlusMillisecond()
        {
            var time = TimeTag.Zero;

            Assert.AreEqual((TimeTag)"[00:00.999]", time.PlusMillisecond(999));
            Assert.AreEqual((TimeTag)"[00:01.0]", time.PlusMillisecond(1000));
            Assert.AreEqual((TimeTag)"[00:01.1]", time.PlusMillisecond(1001));            
        }

        [Test]
        public void ToMillisecond()
        {
            var time_1 = (TimeTag)"[01:01.100]";
            var time_2 = (TimeTag)"[0:00.100]";
            var time_3 = (TimeTag)"[0:01.100]";
            var time_4 = (TimeTag)"[1:00.000]";

            Assert.AreEqual(61100, time_1.ToMillisecond());
            Assert.AreEqual(100, time_2.ToMillisecond());
            Assert.AreEqual(1100, time_3.ToMillisecond());
            Assert.AreEqual(60000, time_4.ToMillisecond());
        }

        [Test]
        public void SubtractTimeTag()
        {
            Assert.AreEqual(TimeTag.Zero, small.SubtractTimeTag(equalSmall));
            Assert.Throws<ArgumentOutOfRangeException>(() => small.SubtractTimeTag(big));
        }

        [Test]
        public void Compare()
        {
            Assert.AreEqual(1, big.CompareTo(small));
            Assert.AreEqual(0, small.CompareTo(equalSmall));
            Assert.AreEqual(-1, small.CompareTo(big));
        }

        // > 运算符
        [Test]
        public void GreaterThan()
        {
            Assert.IsFalse(small > big);
            Assert.IsFalse(small > equalSmall);
            Assert.IsTrue(big > small);
        }

        // < 运算符
        [Test]
        public void LessThan()
        {
            Assert.IsTrue(small < big);
            Assert.IsFalse(big < small);
            Assert.IsFalse(equalSmall < small);
        }

        // <= 运算符
        [Test]
        public void GreaterThanOrEqualTo()
        {
            Assert.IsTrue(small <= big);
            Assert.IsTrue(small <= equalSmall);
            Assert.IsFalse(big <= small);
        }

        // >= 运算符
        [Test]
        public void LessThanOrEqualTo()
        {
            Assert.IsTrue(big >= small);
            Assert.IsTrue(small >= equalSmall);
            Assert.IsFalse(small >= big);
        }

        [Test]
        public void ToTimeTag()
        {
            Assert.AreEqual("[00:00.000]", TimeTag.Zero.ToTimeTag());
            Assert.AreEqual("[01:00.000]", small.ToTimeTag());
            Assert.AreEqual("[02:00.000]", big.ToTimeTag());
            Assert.AreEqual("[02:00.001]", ((TimeTag)"[2:0.1]").ToTimeTag());
        }
    }
}
