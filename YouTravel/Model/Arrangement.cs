using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace YouTravel.Model
{
	public class Arrangement
	{
		[Key] 
		public int Id { get; set; }
		public string Description { get; set; } = "";
		public double Price { get; set; }
		public DateTime Start { get; set; }
		public DateTime End { get; set; }
		public string ImageFname { get; set; } = "";

		public virtual IList<Location> Locations { get; } = new List<Location>();
        public IList<Reservation> Reservations { get; } = new List<Reservation>();
    }
}
