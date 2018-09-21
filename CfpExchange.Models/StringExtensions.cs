namespace CfpExchange.Models
{
    public static class StringExtensions
    {
        public static bool IsDefaultImage(this string urlString)
        {
            return urlString.ToLower().EndsWith("noimage.svg");
        }
    }
}
