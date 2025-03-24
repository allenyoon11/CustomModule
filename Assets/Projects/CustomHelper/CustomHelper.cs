using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace neuroears.allen.utils
{
    public static class CustomHelper
    {
        public static string GenerateRandomNumber(int length)
        {
            string result = "";
            for (int i = 0; i < length; i++)
            {
                int num = UnityEngine.Random.Range(0, 10);
                result += num.ToString();
            }
            return result;
        }
        public static int CalculateAge(DateTime birth)
        {
            DateTime date = DateTime.Now;

            int differenceInYears = date.Year - birth.Year;
            if (date < birth.AddYears(differenceInYears))
            {
                differenceInYears--;
            }
            return differenceInYears;
        }
        public static string GenerateRandomFileName(string ext)
        {
            ext = ext[0] == '.' ? ext.Substring(1) : ext;
            long currentMills = DateTimeHelper.CurrentTimeMillis();
            string randomNum = GenerateRandomNumber(10);
            return string.Format("{0}{1}.{2}", currentMills, randomNum, ext);
        }
        public static string GenerateRandomFileName(string ext, string c)
        {
            ext = ext[0] == '.' ? ext.Substring(1) : ext;
            long currentMills = DateTimeHelper.CurrentTimeMillis();
            string randomNum = GenerateRandomNumber(10);
            return string.Format("{0}{1}{2}.{3}", c, currentMills, randomNum, ext);
        }
    }

}
