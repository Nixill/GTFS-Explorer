using NodaTime;

namespace GTFS_Explorer.Core.Models.Structs
{
    public readonly struct DateRange
    {
        public readonly LocalDate Min;
        public readonly LocalDate Max;

        public DateRange(LocalDate min, LocalDate max)
        {
            Min = min;
            Max = max;
        }

        public static DateRange operator +(DateRange left, DateRange right)
            => new DateRange(
                LocalDate.Min(left.Min, right.Min),
                LocalDate.Max(left.Max, right.Max)
            );
    }
}