using GTFS.Entities;
using GTFS.Entities.Collections;
using System.Collections.Generic;

namespace GTFS_Explorer.Core.Interfaces.RepoInterfaces
{
    public interface IAgencyRepository
    {
        Agency GetFirstAgency();
        IUniqueEntityCollection<Agency> GetAgencies();
    }
}