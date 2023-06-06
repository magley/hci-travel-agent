namespace YouTravel.Util
{
    public static class StringUtil
    {
        public static bool Compare(string s1, string s2)
        {
            s1 = s1.Trim().ToLower();
            s2 = s2.Trim().ToLower();

            return s1 == s2 || s1.Contains(s2) || s2.Contains(s1);
        }
    }
}
