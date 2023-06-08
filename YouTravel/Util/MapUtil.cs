using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using YouTravel.Model;

namespace YouTravel.Util
{
    public class PlacePinData
    {
        public Place Place { get; set; }
        public bool IsSpeculativePin { get; set; }

        public PlacePinData(Place place, bool speculativePin = false)
        {
            Place = place;
            IsSpeculativePin = speculativePin;
        }

        public static IEnumerable<PlacePinData> From(IEnumerable<Place> places, bool speculativePins = false)
        {
            return from pin in places select new PlacePinData(pin, speculativePins);
        }
    }

    public class MapBundle
    {
        public Map? Map { get; set; }
        public IList<PlacePinData> Pins { get; set; }

        public MapBundle(Map map, Place pin)
        {
            Map = map;
            Pins = new List<PlacePinData> { new PlacePinData(pin) };
        }

        public MapBundle()
        {
            Pins = new List<PlacePinData>();
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

            // Pin

            foreach (var pin in bundle.Pins)
            {
                if (pin != null)
                {
                    var location = new Location(pin.Place.Lat, pin.Place.Long);
                    MapLayer mapLayer = new();
                    Image myPushPin = new()
                    {
                        Source = new BitmapImage(new Uri(GetPinIconUriString(pin.Place.Type), UriKind.Absolute)),
                        Width = 48,
                        Height = 48,
                        Opacity = pin.IsSpeculativePin ? 0.5 : 1,
                    };
                    mapLayer.AddChild(myPushPin, location, PositionOrigin.Center);
                    bundle.Map.Children.Add(mapLayer);
                }
            }
        }

        public static void ClearPins(Map map)
        {
            map.Children.Clear();
        }

        public static void DrawPinOnMapBasedOnList(Collection<Place> places, ListBox viewList, Map map, bool moveMapToPin)
        {
            if (places.Count == 0 || viewList.SelectedIndex == -1)
            {
                DrawImage(null, map);
                return;
            }

            var selectedPlace = places[viewList.SelectedIndex];
            DrawPin(selectedPlace, map, moveMapToPin);
        }

        public static void DrawPin(Place place, Map map, bool moveMapToPin)
        {
            var location = new Location(place.Lat, place.Long);
            map.Children.Clear();
            DrawImage(place, map);
            if (moveMapToPin)
            {
                map.Center = location;
            }
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
            mapLayer.AddChild(myPushPin, location, PositionOrigin.BottomCenter);
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
