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
				db.SaveChanges();

				var attr2 = new Location { Name = "DEF's Cave", Type = LocationType.Attraction };
				db.Locations.Add(attr2);
				db.SaveChanges();

				var hotel1 = new Location { Name = "Hotel California", Type = LocationType.Hotel };
				db.Locations.Add(hotel1);
				db.SaveChanges();

				// Query:

				var q = from loc in db.Locations
						where loc.Type == LocationType.Attraction
						select loc;

				Console.WriteLine("Attractions:");
				foreach (var attraction in q)
				{
					Console.WriteLine(attraction.Name);
				}
			}
		}
	}
}
