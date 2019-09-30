using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMS.Service.Repository
{
    public class UtilityRepository
    {

        //string should be in format yyyymmdd
        public static DateTime GettDateTimeFromString(string yyyymmdd)
        {
            if (!string.IsNullOrEmpty(yyyymmdd))
            {
                String modified = yyyymmdd.Insert(4, "-");// "yyyy-mmdd"
                modified = modified.Insert(7, "-"); // "yyyy-mm-dd"
                DateTime oDate = DateTime.Parse(modified);
                return oDate;
            }
            else
            {
                return DateTime.Now;
            }
        }

        public static DateTime UpdatedDateTime (DateTime date)
        {

           DateTime newDate =  date.AddDays(1);
            return newDate;
        }
        //string should be in format yyyymmdd
        public static string GetFormattedDateInMMDDYYYY(string yyyymmdd)
        {
            if (!string.IsNullOrEmpty(yyyymmdd))
            {
                return GettDateTimeFromString(yyyymmdd).ToString("MM/dd/yyyy");
            }
            else
            {
                return DateTime.Now.ToString("MM/dd/yyyy");
            }
        }

        public static DateTime GetTimeZoneCurrentDateTime(string timeZoneName)
        {
            if (!string.IsNullOrEmpty(timeZoneName))
            {
                TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneName);
                DateTime currenttimezonedatetime = TimeZoneInfo.ConvertTime(DateTime.Now, timeZone);
                return currenttimezonedatetime;
            }
            return DateTime.Now;
        }


        public static Tuple<DateTime, string> UtcDateToOtherTimezone(DateTime Date, TimeSpan Time, TimeZoneInfo TimeZoneInformation)
        {
            var UTCDate = new DateTime(Date.Year, Date.Month, Date.Day, Time.Hours, Time.Minutes, Time.Seconds, DateTimeKind.Utc);
            var ConvertedDate = TimeZoneInfo.ConvertTimeFromUtc(UTCDate, TimeZoneInformation);
            var ConvertedTime = GetTimeZoneTime((TimeSpan?)TimeZoneInfo.ConvertTimeFromUtc(UTCDate, TimeZoneInformation).TimeOfDay);
            return Tuple.Create(ConvertedDate, ConvertedTime);
        }

        public static string GetTimeZoneTime(TimeSpan? time)
        {
            if (time.HasValue)
            {
                return time.Value.Hours.ToString("00") + time.Value.Minutes.ToString("00");
            }
            else
            {
                return "";
            }
        }
        public static string GetFlatTimeString(string time)
        {
            if (!string.IsNullOrEmpty(time))
            {
                var str = time.Split(':');
                if (str.Length > 0)
                {
                    return str[0] + str[1];
                }
                return "";
            }
            else
            {
                return "";
            }
        }
        public static string GetFormattedTimeString(string time)
        {
            if (!string.IsNullOrEmpty(time))
            {
                string arr = time.Substring(0, 2);
                string arr1 = time.Substring(2, 2);
                return arr + ":" + arr1;
            }
            else
            {
                return "";
            }
        }


    }
}
