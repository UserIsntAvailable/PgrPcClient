using System.Globalization;

namespace PgrPcClient.Extensions
{
    public static class StringExtensions
    {
        public static bool IsHexValue(this string str) => str.StartsWith("0x", true, CultureInfo.CurrentCulture);
    }
}
