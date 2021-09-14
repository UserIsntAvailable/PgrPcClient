namespace PgrPcClientService.Extensions
{
    public static class StringExtensions
    {
        public static bool IsHexValue(this string str) => str.StartsWith("0x");
    }
}
