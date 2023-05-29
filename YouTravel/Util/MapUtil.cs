using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using YouTravel.Model;

namespace YouTravel.Util
{
	public class MapBundle
	{
		public Map Map { get; set; }
		public IList<Location> RouteLocations { get; set; }
		public Place Pin { get; set; }

		public MapBundle(Map map, IList<Location> routeLocations, Place pin)
		{
			Map = map;
			RouteLocations = routeLocations;
			Pin = pin;
		}

		public MapBundle()
		{
			Map = null;
			RouteLocations = null;
			Pin = null;
		}
	}

    public static class MapUtil
    {
		public static void Redraw(MapBundle bundle)
		{
			if (bundle.Map == null)
			{
				return;
			}

			bundle.Map.Children.Clear();
			
			// Route

			if (bundle.RouteLocations != null)
			{
				var locations = new LocationCollection();
				foreach (var loc in bundle.RouteLocations)
				{
					locations.Add(loc);
				}

				var polyline = new MapPolyline
				{
					Locations = locations,
					Stroke = new SolidColorBrush(Colors.Blue),
					StrokeThickness = 5
				};

				bundle.Map.Children.Add(polyline);
			}

			// Pin

			if (bundle.Pin != null)
			{
				var location = new Location(bundle.Pin.Lat, bundle.Pin.Long);
				MapLayer mapLayer = new();
				Image myPushPin = new()
				{
					Source = new BitmapImage(new Uri(GetPinIconUriString(bundle.Pin.Type), UriKind.Absolute)),
					Width = 48,
					Height = 48
				};
				mapLayer.AddChild(myPushPin, location, PositionOrigin.Center);
				bundle.Map.Children.Add(mapLayer);
			}
		}

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
			map.Children.Clear();
			DrawImage(place, map);
			map.Center = location;
		}

		private static void DrawImage(Place? place, Map map)
		{
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
