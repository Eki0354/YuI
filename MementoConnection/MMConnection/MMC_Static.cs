using System;

namespace MementoConnection
{
    public class ELiteProperties
    {
        public static string DefaultUserName => "佚名";
        public static string BirthDayStringBeforeJudgement => "@#$%^&*(";
        public static string NullErrorMessage => "数据库连接未初始化！";
        public static DateTime BirthdayOfMori => new DateTime(1997, 03, 03);
        public static DateTime JudgementDay => new DateTime(2019, 04, 30);
    }
}
