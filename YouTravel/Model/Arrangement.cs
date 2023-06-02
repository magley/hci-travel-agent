using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace YouTravel.Model
{
	public class Arrangement
	{
		[Key] 
		public int Id { get; set; }
		public string Description { get; set; } = "New Arrangement Description";
		public string Name { get; set; } = "New Arrangement";
		public double Price { get; set; }
		public DateTime Start { get; set; } = DateTime.Now.AddDays(1);
		public DateTime End { get; set; } = DateTime.Now.AddDays(3);
        public string ImageFname { get; set; } = "";

		public IList<Place> Places { get; } = new List<Place>();
        public IList<Reservation> Reservations { get; } = new List<Reservation>();

		public bool IsFinished()
		{
			return End < DateTime.Now;
		}

		public bool IsActive()
		{
			return Start < DateTime.Now && DateTime.Now < End;
		}

		public bool IsUpcoming()
		{
			return DateTime.Now < End;
		}
	}
}
