using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace BiliBiliSubtitlesTool.Lyric
{
    /// <summary>
    /// LRC文件中的时间标签
    /// </summary>
    public readonly struct TimeTag : IComparable<TimeTag>, IComparer<TimeTag>
    {
        public readonly static TimeTag Zero = new TimeTag(0, 0, 0);
        /// <summary>
        /// 分钟最大值
        /// </summary>
        public const byte MINUTE_MAX = 59;
        /// <summary>
        /// 秒最大值
        /// </summary>
        public const byte SECOND_MAX = 59;
        /// <summary>
        /// 毫秒最大值
        /// </summary>
        public const ushort MILLISECOND_MAX = 999;
        /// <summary>
        /// 分钟
        /// </summary>
        private readonly byte _minute;
        /// <summary>
        /// 秒
        /// </summary>
        private readonly byte _second;
        /// <summary>
        /// 毫秒
        /// </summary>
        private readonly ushort _millisecond;

        /// <summary>
        /// 使用指定的分, 秒, 毫秒构建一个对象
        /// </summary>
        /// <param name="newMinute">分</param>
        /// <param name="newSecond">秒</param>
        /// <param name="newMillisecond">毫秒</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public TimeTag(uint newMinute, uint newSecond, uint newMillisecond)
        {
            if (newMinute > MINUTE_MAX)
            {
                throw new ArgumentOutOfRangeException($"{newMinute} > {MINUTE_MAX}");
            }
            if (newSecond > SECOND_MAX)
            {
                throw new ArgumentOutOfRangeException($"{newSecond} > {SECOND_MAX}");
            }
            if (newMillisecond > MILLISECOND_MAX)
            {
                throw new ArgumentOutOfRangeException($"{newMillisecond} > {MILLISECOND_MAX}");
            }
            _minute = (byte)newMinute;
            _second = (byte)newSecond;
            _millisecond = (ushort)newMillisecond;
        }

        /// <summary>
        /// 使用字符串构建一个时间标签
        /// </summary>
        /// <param name="line">字符串形式的时间标签</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="FormatException"></exception>
        public TimeTag(string line)
        {
            if (line == null)
                throw new ArgumentNullException(nameof(line));
            if (!Regex.IsMatch(line, "\\[\\d{1,2}:\\d{1,2}\\.\\d{1,3}\\]"))
                throw new FormatException();
            
            //去除歌词和[]
            string timeTag = line.Split(new char[] {'[', ']'})[1];
            string[] timeArray = timeTag.Split(new char[] {':', '.'});
            string millisecondString;
            try
            {
                _minute = byte.Parse(timeArray[0]);
                _second = byte.Parse(timeArray[1]);
                millisecondString = timeArray[2];
                _millisecond = ushort.Parse(millisecondString);
            }
            catch (FormatException)
            {
                throw;
            }
            if (millisecondString.Length == 2)
                _millisecond *= 10;
            if (_minute > MINUTE_MAX || _second > SECOND_MAX || _millisecond > MILLISECOND_MAX)
                throw new FormatException("时间超过最大值");
        }

        /// <summary>
        /// 分钟
        /// </summary>
        public uint Minute
        {
            get => _minute;
        }

        /// <summary>
        /// 秒
        /// </summary>
        public uint Second
        {
            get => _second;
        }

        /// <summary>
        /// 毫秒
        /// </summary>
        public uint Millisecond
        {
            get => _millisecond;
        }

        /// <summary>
        /// 返回一个标准的时间标签
        /// </summary>
        /// <returns>一个标准的时间标签</returns>
        public string ToTimeTag()
        {
            return string.Format("[{0:d2}:{1:d2}.{2:d3}]", Minute, Second, Millisecond);
        }

        /// <summary>
        /// 得到Bili字幕格式的时间标签
        /// </summary>
        /// <returns></returns>
        public double GetBiliFormatTimeTag()
        {
            double num = 0;
            if (_millisecond != 0)
            {
                num = (double)_millisecond / 1000;
            }
            return _minute*60 + _second + Math.Round(num, 3);
        }

        /// <summary>
        /// 返回基于this加上毫秒的副本
        /// </summary>
        /// <param name="addMillisecond">增加的毫秒</param>
        /// <returns>基于this加上毫秒的副本</returns>
        /// <exception cref="ArgumentException">如果时间过大</exception>
        /// <date>2021-10-1</date>
        public TimeTag PlusMillisecond(in uint addMillisecond)
        {
            uint newMillisecond = Millisecond + addMillisecond;
            uint newMinute = Minute;
            uint newSecond = Second;

            while (newMillisecond > MILLISECOND_MAX)
            {
                newMillisecond -= 1000;
                ++newSecond;
                if (newSecond > SECOND_MAX)
                {
                    ++newMinute;
                    newSecond -= 60;
                }
            }
            if (newMinute > MINUTE_MAX)
            {
                throw new ArgumentException($"{newMinute} > {MINUTE_MAX}");
            }
            return new TimeTag(newMinute, newSecond, newMillisecond);
        }

        /// <summary>
        /// 返回this减去<c>time</c>的值
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public TimeTag SubtractTimeTag(in TimeTag time)
        {
            if (time > this)
            {
                throw new ArgumentOutOfRangeException($"{nameof(time)} > this");
            }
            if (time == this)
            {
                return Zero;
            }
            return Zero.PlusMillisecond(this.ToMillisecond() - time.ToMillisecond());
        }

        public override string ToString()
        {
            return string.Format("{{分={0:d2} 秒={1:d2} 毫秒={2:d3} }}", Minute, Second, Millisecond);            
        }

        /// <summary>
        /// 返回总和的毫秒
        /// </summary>
        /// <returns>换算成毫秒的数</returns>
        public uint ToMillisecond()
        {
            return (Minute * 60 + Second) * 1000 + Millisecond;
        }
        #region 相等性, 散列码
        public override bool Equals(object obj)
        {
            return obj is TimeTag tag &&
                   _minute == tag._minute &&
                   _second == tag._second &&
                   _millisecond == tag._millisecond;
        }

        public override int GetHashCode()
        {
            int hash = _minute;
            hash = hash * 31 + _second;
            hash = hash * 31 + _millisecond;
            return hash;
        }

        public int Compare(TimeTag x, TimeTag y)
        {
            if (x == y)
            {
                return 0;
            }

            return x.ToMillisecond().CompareTo(y.ToMillisecond());           
        }

        public int CompareTo(TimeTag x)
        {
            return Compare(this, x);
        }
        #endregion

        #region 运算符重载

        public static TimeTag operator +(in TimeTag left, in TimeTag right)
        {
            return left.PlusMillisecond(right.ToMillisecond());
        }
        
        public static TimeTag operator -(in TimeTag left, in TimeTag right)
        {
            return left.SubtractTimeTag(right);
        }

        public static bool operator ==(in TimeTag left, in TimeTag right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(in TimeTag left, in TimeTag right)
        {
            return !(left == right);
        }

        public static bool operator >(in TimeTag left, in TimeTag right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator <(in TimeTag left, in TimeTag right)
        {
            if (left == right)
            {
                return false;
            }
            return !(left > right);
        }

        public static bool operator >=(in TimeTag left, in TimeTag right)
        {
            return !(left < right);
        }

        public static bool operator <=(in TimeTag left, in TimeTag right)
        {
            return !(left > right);
        }

        public static explicit operator TimeTag(string timeTag)
        {
            return new TimeTag(timeTag);
        }
        #endregion
    }
}
