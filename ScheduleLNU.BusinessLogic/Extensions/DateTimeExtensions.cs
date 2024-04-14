using System;

namespace ScheduleLNU.BusinessLogic.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime RemoveSeconds(this DateTime dateTime)
        {
            return dateTime.AddSeconds(-dateTime.Second);
        }
    }
}
