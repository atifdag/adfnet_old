using System;

namespace ADF.Net.Core.Helpers
{
    public static class GuidHelper
    {
        private static readonly long BaseDateTicks = new DateTime(1900, 1, 1).Ticks;

        public static Guid NewGuid()
        {
            var bytesGuid = Guid.NewGuid().ToByteArray();
            var now = DateTime.UtcNow;
            var days = new TimeSpan(now.Ticks - BaseDateTicks);
            var daysArray = BitConverter.GetBytes(days.Days);
            var msecs = BitConverter.GetBytes((long)(now.TimeOfDay.TotalMilliseconds / 3.333333));
            Array.Reverse(daysArray);
            Array.Reverse(msecs);
            Array.Copy(daysArray, daysArray.Length - 2, bytesGuid, bytesGuid.Length - 6, 2);
            Array.Copy(msecs, msecs.Length - 4, bytesGuid, bytesGuid.Length - 4, 4);
            return new Guid(bytesGuid);
        }

    }
}
