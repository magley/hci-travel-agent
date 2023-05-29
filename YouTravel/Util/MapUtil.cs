using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using YouTravel.Model;

namespace YouTravel.Util
{
    public static class MapUtil
    {
		public static void DrawPinOnMapBasedOnList(Collection<Place> places, ListBox viewList, Map map)
		{
			if (places.Count == 0 || viewList.SelectedIndex == -1)
			{
				DrawImage(null, map);
				return;
			}

			var selectedPlace = places[viewList.SelectedIndex];
			DrawPin(selectedPlace, map);
		}

		public static void DrawPin(Place place, Map map)
		{
			var location = new Location(place.Lat, place.Long);
			DrawImage(place, map);
			map.Center = location;
		}

		private static void DrawImage(Place? place, Map map)
		{
			map.Children.Clear();

			if (place == null)
			{
				// Setting place to null removes the pin.
				return;
			}

			var location = new Location(place.Lat, place.Long);
			MapLayer mapLayer = new();
			Image myPushPin = new()
			{
				Source = new BitmapImage(new Uri(GetPinIconUriString(place.Type), UriKind.Absolute)),
				Width = 48,
				Height = 48
			};
			mapLayer.AddChild(myPushPin, location, PositionOrigin.Center);
			map.Children.Add(mapLayer);
		}

		private static string GetPinIconUriString(PlaceType type)
		{
			string fname = "";
			switch (type)
			{
				case PlaceType.Attraction:
					fname = "ImgAttraction.png";
					break;
				case PlaceType.Restaurant:
					fname = "ImgRestaurant.png";
					break;
				case PlaceType.Hotel:
					fname = "ImgHotel.png";
					break;
			}

			return $"pack://application:,,,/Res/{fname}";
		}
	}
}
