using System.Collections.Generic;
using GTFS;
using GTFS.Entities;
using Nixill.Collections;

namespace GTFS_Explorer.BackEnd.Lookups
{
  public static class Lists
  {
    /// <summary>
    /// Returns a <c>Dictioary</c> containing all the routes in the feed,
    /// separated by agency.
    /// </summary>
    /// <param name="feed">The GTFS feed to use.</param>
    public static Dictionary<Agency, List<Route>> GetAllRoutes(GTFSFeed feed)
    {
      GeneratorDictionary<Agency, List<Route>> dict = new GeneratorDictionary<Agency, List<Route>>(new FuncGenerator<Agency, List<Route>>(x => new List<Route>()));

      var agencies = feed.Agencies;

      foreach (Route route in feed.Routes)
      {
        dict[agencies.Get(route.AgencyId)].Add(route);
      }

      return dict;
    }

    /// <summary>
    /// Returns a <c>Dictionary</c> containing all the stops in the feed,
    /// along with how major a stop it is.
    /// </summary>
    public static Dictionary<Stop, StopMajority> GetAllStops(GTFSFeed feed)
    {
      Dictionary<Stop, StopMajority> dict = new Dictionary<Stop, StopMajority>();

      var stops = feed.Stops;

      return null;
    }
  }

  public enum StopMajority {
    All,
    Some,
    None
  }
}
