using System;

namespace ManseCalendarIntegrated
{
    public class Program
    {
        static void Main()
        {
            // 입력 샘플: 로컬 시각 + 타임존 오프셋
            // 예: 1076-12-14 11:30, 한국 UTC+9
            DateTime localInput = new(1976, 12, 14, 11, 00, 0, DateTimeKind.Unspecified);
            TimeSpan tzOffset = TimeSpan.FromHours(9);

            var result = ManseCalendar.ComputeManse(localInput, tzOffset);

            Console.WriteLine($"입력(로컬) : {localInput:yyyy-MM-dd HH:mm} (UTC{tzOffset.TotalHours:+0;-0})");
            Console.WriteLine($"UTC 시간   : {result.UtcTime:u}");
            Console.WriteLine();

            Console.WriteLine($"음력       : {result.LunarYear}년 {(result.IsLeapMonth ? "윤" : "")}{result.LunarMonth}월 {result.LunarDay}일");
            Console.WriteLine($"년주       : {result.YearGanJi}");
            Console.WriteLine($"월주       : {result.MonthGanJi}");
            Console.WriteLine($"일주       : {result.DayGanJi}");
            Console.WriteLine($"시주       : {result.HourGanJi}");
            Console.WriteLine();
            Console.WriteLine($"가장 가까운 절기 : {result.NearestSolarTermName}");
            Console.WriteLine($"절기 시각 (UTC)  : {result.NearestSolarTermUtc:u}");
            Console.WriteLine($"절기 시각 (Local) : {(result.NearestSolarTermUtc + tzOffset):yyyy-MM-dd HH:mm}");
        }
    }
}
