using GTFS.Entities;
using GTFS.Entities.Collections;
using GTFS_Explorer.BackEnd.Readers;
using GTFS_Explorer.Core.Interfaces.RepoInterfaces;
using System.Collections.Generic;
using System.Linq;

namespace GTFS_Explorer.BackEnd.Repositiories
{
    public class AgencyRepository : IAgencyRepository
    {
        public GTFSFeedReader _reader;

        public AgencyRepository(GTFSFeedReader reader)
        {
            _reader = reader;
        }

        public IUniqueEntityCollection<Agency> GetAgencies()
        {
            return _reader.Feed.Agencies;
        }

        public Agency GetFirstAgency()
        {
            return _reader.Feed.Agencies.FirstOrDefault();
        }
    }
}