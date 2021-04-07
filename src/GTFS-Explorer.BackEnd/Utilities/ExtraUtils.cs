using GTFS.Entities;
using NodaTime;
using NodaTime.Text;

namespace GTFS_Explorer.BackEnd.Utilities
{
  public static class ExtraUtils
  {
    public static Duration DurationFromTimeOfDay(TimeOfDay time)
    {
      return Duration.FromSeconds(time.TotalSeconds);
    }
  }
}