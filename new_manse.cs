using System;

public class KoreanManseCalculator
{
    static readonly string[] 천간 = { "갑", "을", "병", "정", "무", "기", "경", "신", "임", "계" };
    static readonly string[] 지지 = { "자", "축", "인", "묘", "진", "사", "오", "미", "신", "유", "술", "해" };
    static readonly string[] 절기 = {
        "입춘", "우수", "경칩", "춘분", "청명", "곡우",
        "입하", "소만", "망종", "하지", "소서", "대서",
        "입추", "처서", "백로", "추분", "한로", "상강",
        "입동", "소설", "대설", "동지", "소한", "대한"
    };

    public static void Main()
    {
        // 입력값 예시
        DateTime solarDate = new DateTime(2025, 10, 16, 14, 0, 0); // UTC+9 기준

        // 절기
        string currentTerm = GetSolarTerm(solarDate);

        // 음력 변환
        var lunarDate = SolarToLunar(solarDate);

        // 간지
        string yearGanji = GetYearGanji(lunarDate.Year);
        string monthGanji = GetMonthGanji(lunarDate.Year, lunarDate.Month);
        string dayGanji = GetDayGanji(solarDate);
        string hourGanji = GetHourGanji(dayGanji, solarDate.Hour);

        Console.WriteLine($"양력: {solarDate:yyyy-MM-dd HH:mm}");
        Console.WriteLine($"음력: {lunarDate:yyyy-MM-dd}");
        Console.WriteLine($"절기: {currentTerm}");
        Console.WriteLine($"년주: {yearGanji}, 월주: {monthGanji}, 일주: {dayGanji}, 시주: {hourGanji}");
    }

    // -----------------------------------------------------
    // 1️⃣ 음력 계산 (근사)
    // -----------------------------------------------------
    static DateTime SolarToLunar(DateTime solar)
    {
        // 1900년 1월 31일 기준 음력 1월 1일
        DateTime baseDate = new DateTime(1900, 1, 31);
        int offsetDays = (solar - baseDate).Days;
        int year = 1900;

        // 각 해의 음력 일수 테이블 (간략)
        int[] lunarInfo = {
            0x04bd8,0x04ae0,0x0a570,0x054d5,0x0d260,0x0d950,
            0x16554,0x056a0,0x09ad0,0x055d2,0x04ae0,0x0a5b6,
            0x0a4d0,0x0d250,0x1d255,0x0b540,0x0d6a0,0x0ada2,
            0x095b0,0x14977,0x04970,0x0a4b0,0x0b4b5,0x06a50,
            0x06d40,0x1ab54,0x02b60,0x09570,0x052f2,0x04970,
            0x06566,0x0d4a0,0x0ea50,0x06e95,0x05ad0,0x02b60,
            0x186e3,0x092e0,0x1c8d7,0x0c950,0x0d4a0,0x1d8a6,
            0x0b550,0x056a0,0x1a5b4,0x025d0,0x092d0,0x0d2b2,
            0x0a950,0x0b557,0x06ca0,0x0b550,0x15355,0x04da0,
            0x0a5d0,0x14573,0x052d0,0x0a9a8,0x0e950,0x06aa0,
            0x0aea6,0x0ab50,0x04b60,0x0aae4,0x0a570,0x05260,
            0x0f263,0x0d950,0x05b57,0x056a0,0x096d0,0x04dd5,
            0x04ad0,0x0a4d0,0x0d4d4,0x0d250,0x0d558,0x0b540,
            0x0b5a0,0x195a6,0x095b0,0x049b0,0x0a974,0x0a4b0,
            0x0b27a,0x06a50,0x06d40,0x0af46,0x0ab60,0x09570,
            0x04af5,0x04970,0x064b0,0x074a3,0x0ea50,0x06b58,
            0x05ac0,0x0ab60,0x096d5,0x092e0,0x0c960,0x0d954,
            0x0d4a0,0x0da50,0x07552,0x056a0,0x0abb7,0x025d0,
            0x092d0,0x0cab5,0x0a950,0x0b4a0,0x0baa4,0x0ad50,
            0x055d9,0x04ba0,0x0a5b0,0x15176,0x052b0,0x0a930,
            0x07954,0x06aa0,0x0ad50,0x05b52,0x04b60,0x0a6e6,
            0x0a4e0,0x0d260,0x0ea65,0x0d530,0x05aa0,0x076a3,
            0x096d0,0x04bd7,0x04ad0,0x0a4d0,0x1d0b6,0x0d250,
            0x0d520,0x0dd45,0x0b5a0,0x056d0,0x055b2,0x049b0,
            0x0a577,0x0a4b0,0x0aa50,0x1b255,0x06d20,0x0ada0
        };

        while (offsetDays >= GetLunarYearDays(year, lunarInfo))
        {
            offsetDays -= GetLunarYearDays(year, lunarInfo);
            year++;
        }

        int leapMonth = GetLeapMonth(lunarInfo[year - 1900]);
        bool isLeap = false;
        int month = 1;
        int daysInMonth;

        while (true)
        {
            daysInMonth = GetLunarMonthDays(year, month, lunarInfo);
            if (offsetDays < daysInMonth) break;
            offsetDays -= daysInMonth;

            if (leapMonth > 0 && month == leapMonth)
            {
                if (!isLeap)
                {
                    isLeap = true;
                    continue;
                }
                isLeap = false;
            }

            month++;
        }

        int day = offsetDays + 1;
        return new DateTime(year, month, day);
    }

    static int GetLunarYearDays(int year, int[] lunarInfo)
    {
        int sum = 348;
        int info = lunarInfo[year - 1900];
        for (int i = 0x8000; i > 0x8; i >>= 1)
            sum += (info & i) != 0 ? 1 : 0;
        return sum + GetLeapDays(year, lunarInfo);
    }

    static int GetLeapMonth(int info) => info & 0xf;
    static int GetLeapDays(int year, int[] lunarInfo) =>
        (lunarInfo[year - 1900] & 0xf0000) != 0 ? 30 : (GetLeapMonth(lunarInfo[year - 1900]) != 0 ? 29 : 0);
    static int GetLunarMonthDays(int year, int month, int[] lunarInfo) =>
        (lunarInfo[year - 1900] & (0x10000 >> month)) != 0 ? 30 : 29;

    // -----------------------------------------------------
    // 2️⃣ 절기 계산 (근사식)
    // -----------------------------------------------------
    static string GetSolarTerm(DateTime date)
    {
        // 1년 365.2422일 기준, 입춘(2월 4일)을 0도로 설정
        double yearProgress = (date - new DateTime(date.Year, 2, 4)).TotalDays / 365.2422 * 360;
        int termIndex = (int)Math.Floor((yearProgress + 15) / 15.0) % 24;
        return 절기[termIndex];
    }

    // -----------------------------------------------------
    // 3️⃣ 간지 계산
    // -----------------------------------------------------
    static string GetYearGanji(int year)
    {
        int gan = (year - 4) % 10;
        int ji = (year - 4) % 12;
        return $"{천간[gan]}{지지[ji]}년";
    }

    static string GetMonthGanji(int year, int month)
    {
        int index = ((year - 1984) * 12 + month - 1) % 60;
        return $"{천간[index % 10]}{지지[index % 12]}월";
    }

    static string GetDayGanji(DateTime date)
    {
        DateTime baseDate = new DateTime(1984, 2, 2);
        int diff = (int)(date - baseDate).TotalDays;
        return $"{천간[(diff % 10 + 10) % 10]}{지지[(diff % 12 + 12) % 12]}일";
    }

    static string GetHourGanji(string dayGanji, int hour)
    {
        int dayGanIndex = Array.IndexOf(천간, dayGanji.Substring(0, 1));
        int hourIndex = (int)Math.Floor((hour + 1) / 2.0) % 12;
        int ganIndex = (dayGanIndex * 2 + hourIndex) % 10;
        return $"{천간[ganIndex]}{지지[hourIndex]}시";
    }
}
