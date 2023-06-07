using System;
using System.Linq;
using System.Windows;
using YouTravel.Model;
using YouTravel.Util;

namespace YouTravel
{
	public partial class App : Application
	{
		public App()
		{
			FakeMain();
		}

		private static void FakeMain()
		{
			UserConfig.Instance.LoadToolbarConfig();

			using (var db = new TravelContext())
			{
				// Add:

				var attr1 = new Place { Name = "Mt. ABC", Type = PlaceType.Attraction, Lat=45, Long=19 };
				db.Places.Add(attr1);

				var attr2 = new Place { Name = "DEF's Cave", Type = PlaceType.Attraction, Lat = 45.1, Long = 19.2 };
				db.Places.Add(attr2);

				var hotel1 = new Place { Name = "Hotel California", Type = PlaceType.Hotel, Lat = 45.3, Long = 18.7 };
				db.Places.Add(hotel1);

				var arr1 = new Arrangement { Name = "Upcoming Arrangement", Price = 123, Start = DateTime.Now.AddDays(2), End = DateTime.Now.AddDays(4) };
				arr1.Places.Add(attr1);
				arr1.Places.Add(hotel1);
				db.Arrangements.Add(arr1);

				var arr2 = new Arrangement { Name = "Travel in Progress", Price = 356, Start = DateTime.Now.AddDays(-2), End = DateTime.Now.AddDays(2) };	
				arr2.Places.Add(attr1);
				arr2.Places.Add(attr2);
				db.Arrangements.Add(arr2);

				var arr3 = new Arrangement { Name = "Old, Finished Arrangement", Price = 500, Start = DateTime.Now.AddDays(-2), End = DateTime.Now.AddDays(-1) };
				arr3.Places.Add(attr1);
				arr3.Places.Add(attr2);
				arr3.Places.Add(hotel1);
				db.Arrangements.Add(arr3);

				var arrangements = db.Arrangements.Local.ToList();
				var rnd = new Random(1);
				for (int i = 0; i < 20; ++i)
				{
					var timeOfReservation = DateTime.Now.AddMonths(rnd.Next(-3, 1));
					var arr = arrangements[rnd.Next(arrangements.Count)];
					var res = new Reservation { TimeOfReservation = timeOfReservation, Arrangement = arr, Username = "user", NumOfPeople = (i + 1) % 4, PaidOn = (i % 2 == 0) ? timeOfReservation : null };
					db.Reservations.Add(res);
				}

				db.SaveChanges();

				Console.WriteLine("Opening window...");

				// Query:

				//var q = from loc in db.Places
				//		where loc.Type == PlaceType.Attraction
				//		select loc;
				//var q2 = from arr in db.Arrangements
				//		 where arr.Places.Where(loc => loc.Type == PlaceType.Hotel).Count() > 0
				//		 select arr;

				//Console.WriteLine("\nAttractions:");
				//foreach (var attraction in q)
				//{
				//	Console.WriteLine(attraction.Name);
				//}

				//Console.WriteLine("\nArrangements with a hotel:");
				//foreach (var arrangement in q2)
				//{
				//	Console.WriteLine(arrangement.Description);
				//}
			}
		}
	}
}
