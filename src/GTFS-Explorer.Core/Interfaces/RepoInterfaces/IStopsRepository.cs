using GTFS.Entities;
using GTFS_Explorer.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace GTFS_Explorer.Core.Interfaces.RepoInterfaces
{
    public interface IStopsRepository
    {
        Dictionary<Stop, StopMajority> GetAllStops();
    }
}
