using Microsoft.EntityFrameworkCore;

namespace YouTravel.Model
{
	public class TravelContext : DbContext
	{
		public DbSet<Arrangement> Arrangements { get; set; }
		public DbSet<Place> Places { get; set; }
		public DbSet<Reservation> Reservations { get; set; }

		private static bool _created = false;
		public TravelContext()
		{
			// https://www.talkingdotnet.com/create-sqlite-db-entity-framework-core-code-first/

			if (!_created)
			{
				_created = true;
				Database.EnsureDeleted();
				Database.EnsureCreated();
			}
		}

		protected override void OnConfiguring(DbContextOptionsBuilder options)
		{
			options.UseSqlite("Data Source=Data/app.db");
		}
	}
}
