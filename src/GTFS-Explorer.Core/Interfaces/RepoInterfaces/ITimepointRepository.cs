using GTFS.Entities.Enumerations;
using GTFS_Explorer.Core.Enums;
using System;
using System.Collections.Generic;

namespace GTFS_Explorer.Core.Interfaces.RepoInterfaces
{
    public interface ITimepointRepository
    {
        TimepointStrategy GetTimepointStrategy();
        IEnumerable<Tuple<string, bool>> DataTimepoints(string route, DirectionType? dir, bool strict = false);
        IEnumerable<string> FirstStops(string route, DirectionType? dir);
        IEnumerable<string> LastStops(string route, DirectionType? dir);
        IEnumerable<Tuple<string, string>> FirstAndLastStopPairs(string route, DirectionType? dir);
        IEnumerable<string> FirstAndLastStopList(string route, DirectionType? dir);
    }
}