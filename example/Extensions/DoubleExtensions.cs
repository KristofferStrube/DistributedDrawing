using System.Globalization;

namespace DistributedDrawing.Example.Extensions
{
    public static class DoubleExtensions
    {
        public static string AsString(this double d) => d.ToString(CultureInfo.InvariantCulture);
    }
}
