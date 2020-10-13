namespace Adfnet.Core.Constants
{
    public static class EmailConstants
    {
        public static string EmailKeyRegEx => @"\{\%[<" + EmailTokenName + @">]*\%\}";
        public static string EmailTokenName => "EmailTokenName";
    }
}