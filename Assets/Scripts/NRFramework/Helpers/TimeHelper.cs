using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace NRFramework
{
    /// <summary>
    /// 时间工具类
    /// </summary>
    static public class TimeHelper
    {
        static private double m_DeltaSeconds;  //服务器与客户端时间差
        static private readonly DateTime sm_UtcZeroTime;

        static TimeHelper()
        {
            m_DeltaSeconds = 0;
            sm_UtcZeroTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        }
        
        //服务器utc时间（推算的）
        static public DateTime serverUtcNow { get { return utcNow.AddSeconds(m_DeltaSeconds); } }

        //服务器utc时间戳（推算的）
        static public double serverUtcTimestamp { get { return DateTimeToTimestamp(serverUtcNow); } }

        //客户端utc时间
        static public DateTime utcNow { get { return DateTime.UtcNow; } }

        //客户端utc时间戳
        static public double utcTimestamp { get { return DateTimeToTimestamp(DateTime.UtcNow); } }

        //客户端当地时区时间
        static public DateTime localNow { get { return DateTime.Now; } }

        /// <summary>
        /// 时间同步
        /// </summary>
        /// <param name="serverTimestamp">服务器Utc时间戳</param>
        static public void Sync(double serverTimestamp)
        {
            m_DeltaSeconds = serverTimestamp - utcTimestamp;
        }

        /// <summary>
        /// DateTime 转为 Timestamp（时间戳始终为Utc绝对零时经过的秒数）
        /// </summary>
        static public double DateTimeToTimestamp(DateTime dateTime, int timeZone = 0)
        {
            return (DateTimeToUtcDateTime(dateTime, timeZone) - sm_UtcZeroTime).TotalSeconds;
        }

        /// <summary>
        /// DateTime 转为 Timestamp（时间戳始终为Utc绝对零时经过的秒数）
        /// </summary>
        static public DateTime TimestampToDateTime(double timestamp, int timeZone = 0)
        {
            return UtcDateTimeToDateTime(sm_UtcZeroTime, timeZone).AddSeconds(timestamp);
        }

        /// <summary>
        /// UtcDateTime 转为 DateTime
        /// </summary>
        static public DateTime UtcDateTimeToDateTime(DateTime utcDateTime, int timeZone)
        {
            return utcDateTime.AddHours(timeZone);
        }

        /// <summary>
        /// DateTime 转为 UtcDateTime
        /// </summary>
        /// <returns></returns>
        static public DateTime DateTimeToUtcDateTime(DateTime dateTime, int timeZone)
        {
            return dateTime.AddHours(-timeZone);
        }
    }
}
