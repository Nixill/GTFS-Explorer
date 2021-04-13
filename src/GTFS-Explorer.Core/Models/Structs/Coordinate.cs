namespace GTFS_Explorer.Core.Models.Structs
{
    public readonly struct Coordinate
    {
        public readonly double Latitude;
        public readonly double Longitude;
        public Coordinate(double lat, double lon)
        {
            Latitude = lat;
            Longitude = lon;
        }
        public override int GetHashCode()
        {
            return Latitude.GetHashCode() ^ Longitude.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            if (!(obj is Coordinate right)) return false;
            return (Latitude == right.Latitude && Longitude == right.Longitude);
        }
        public static bool operator ==(Coordinate left, Coordinate right) => left.Equals(right);
        public static bool operator !=(Coordinate left, Coordinate right) => !(left.Equals(right));
    }
}