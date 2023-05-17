using System.ComponentModel.DataAnnotations;

namespace YouTravel.Model
{
	public enum LocationType
	{
		Attraction, Restaurant, Hotel
	}

	public class Location
	{
		[Key]
		public int Id { get; set; }
		public string Name { get; set; }
		public double Lat { get; set; }
		public double Long { get; set; }
		public LocationType Type { get; set; }
	}
}
