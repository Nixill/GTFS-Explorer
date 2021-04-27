using System;
using static System.Math;

namespace GTFS_Explorer.BackEnd.Lookups
{
    public static class GIS
    {
        public const double EarthRadiusMeters = 6_378_137;
        public const double D2R = PI / 180; // degrees to radians factor: angle in degrees times D2R = angle in radians

        public static double MetersBetween(double dLat1, double dLon1, double dLat2, double dLon2)
        {
            double rLat1 = dLat1 * D2R;
            double rLat2 = dLat2 * D2R;
            double rLon1 = dLon1 * D2R;
            double rLon2 = dLon2 * D2R;

            double ΔLat = rLat2 - rLat1;
            double ΔLon = rLon2 - rLon1;

            double a = Sin(ΔLat / 2) * Sin(ΔLat / 2) +
                       Cos(rLat1) * Cos(rLat2) *
                       Sin(ΔLon / 2) * Sin(ΔLon / 2);
            double c = 2 * Atan2(Sqrt(a), Sqrt(1 - a));

            return EarthRadiusMeters * c; // in metres
        }

        public static double DegreesInOneKm(double dLat) => Atan2(Sin(1000 / EarthRadiusMeters), Cos(1000 / EarthRadiusMeters) - Sin(dLat * D2R) * Sin(dLat * D2R)) / D2R;
    }
}