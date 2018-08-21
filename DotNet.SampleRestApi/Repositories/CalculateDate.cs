using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNet.SampleRestApi.Repositories
{
    public class CalculateDate
    {
        int leap(int year)
        {
            if ((year % 4 == 0 && year % 100 != 0) || year % 400 == 0)
            {
                return 1;   // Leap year (29)
            }
            else
            {
                return 0;   // usual year (28)
            }
        }
        public int calculate(String beginDate, String endDate)
        {
            int a, b, day, month, year, weekday, ans;
            int[] bDate = beginDate.Split('.').Select(int.Parse).ToArray();
            int[] eDate = endDate.Split('.').Select(int.Parse).ToArray();
            int beginDay = bDate[0];
            int beginMonth = bDate[1];
            int beginYear = bDate[2];

            int endDay = eDate[0];
            int endMonth = eDate[1];
            int endYear = eDate[2];
            day = 1;
            month = 1;
            weekday = 1;
            year = 1900;
            ans = 0;
            while (true)
            {
                if ((year > beginYear) || (year == beginYear && month > beginMonth) || ((year == beginYear && month == beginMonth && day >= beginDay)))
                {
                    if ((year < endYear) || (year == endYear && month < endMonth) || (year == endYear && month == endMonth && day <= endDay))
                    {
                        if (weekday == 7)
                        {
                            ans++;
                            Console.WriteLine(day + " " + month + " " + year);
                        }
                    }
                }


                if ((year > endYear) || (year == endYear && month > endMonth) || (year == endYear && month == endMonth && day > endDay))
                {
                    break;
                }

                if (month == 1 || month == 3 || month == 5 || month == 7 || month == 8 || month == 10 || month == 12)
                {
                    weekday += 3;
                }
                else if (month == 4 || month == 6 || month == 9 || month == 11)
                {
                    weekday += 2;
                }
                else if (month == 2 && leap(year) == 1)
                {
                    weekday += 1;
                }

                month += 1;
                if (weekday > 7)
                {
                    weekday -= 7;
                }
                if (month > 12)
                {
                    month -= 12;
                    year++;
                }
            }
            return ans;
        }
    }
}
