using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace neuroears.allen.utils
{
    public static class DateTimeHelper
    {
        /// <summary>
        /// System.currentTimeMillis() in JAVA
        /// </summary>
        /// <returns></returns>
        public static long CurrentTimeMillis()
        {
            DateTime Jan1st1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            DateTimeOffset now = DateTimeOffset.Now;
            return (long)(now - Jan1st1970).TotalMilliseconds;
        }
    }


}
