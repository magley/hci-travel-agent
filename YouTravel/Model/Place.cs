using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace YouTravel.Model
{
    public enum PlaceType
    {
        Attraction, Restaurant, Hotel
    }

    public class Place
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public double Lat { get; set; }
        public double Long { get; set; }
        public PlaceType Type { get; set; }

        public IList<Arrangement> Arrangements { get; } = new List<Arrangement>();
    }
}
