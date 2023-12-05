
using System.Text.RegularExpressions;

namespace FrontToBack.Utilities.Extentions
{
    public static class Stringformat
    {
        public static string Capitalize(this string name)
        {
            name = name.Substring(0, 1).ToUpper() + name.Substring(1).ToLower();
            return name;
        }
        public static bool IsDigit(this string name)
        {
            if ((name.Any(char.IsDigit)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool CheckEmail(this string email)
        {
            if (string.IsNullOrEmpty(email))
                return false;

            string emailregex = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            Regex regex = new Regex(emailregex);

            return regex.IsMatch(email);
        }


    }
}
