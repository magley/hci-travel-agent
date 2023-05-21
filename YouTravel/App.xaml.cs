using System;
using System.Data;
using System.Linq;
using System.Windows;
using YouTravel.Model;

namespace YouTravel
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		public App()
		{
			FakeMain();
		}

		private static void FakeMain()
		{
			using (var db = new TravelContext())
			{
				// Add:

				var attr1 = new Location { Name = "Mt. ABC", Type = LocationType.Attraction };
				db.Locations.Add(attr1);

				var attr2 = new Location { Name = "DEF's Cave", Type = LocationType.Attraction };
				db.Locations.Add(attr2);

				var hotel1 = new Location { Name = "Hotel California", Type = LocationType.Hotel };
				db.Locations.Add(hotel1);

				var arr1 = new Arrangement { Description = "Arrangement 1", Price=123 };
				arr1.Locations.Add(attr1);
				arr1.Locations.Add(hotel1);
				db.Arrangements.Add(arr1);

				var arr2 = new Arrangement { Description = "Arrangement 2", Price = 356 };	
				arr2.Locations.Add(attr1);
				arr2.Locations.Add(attr2);
				db.Arrangements.Add(arr2);

				var arr3 = new Arrangement { Description = "Arrangement 3", Price = 500 };
				arr3.Locations.Add(attr1);
				arr3.Locations.Add(attr2);
				arr3.Locations.Add(hotel1);
				db.Arrangements.Add(arr3);

				var res1 = new Reservation { TimeOfReservation = DateTime.Now, Arrangement = arr1 };
                var res2 = new Reservation { TimeOfReservation = DateTime.Now, Arrangement = arr1 };
                var res3 = new Reservation { TimeOfReservation = DateTime.Now, Arrangement = arr1 };
                var res4 = new Reservation { TimeOfReservation = DateTime.Now, Arrangement = arr2 };

                db.Reservations.Add(res1);
                db.Reservations.Add(res2);
                db.Reservations.Add(res3);
                db.Reservations.Add(res4);

				db.SaveChanges();

				Console.WriteLine("Opening window...");

				// Query:

				//var q = from loc in db.Locations
				//		where loc.Type == LocationType.Attraction
				//		select loc;
				//var q2 = from arr in db.Arrangements
				//		 where arr.Locations.Where(loc => loc.Type == LocationType.Hotel).Count() > 0
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
