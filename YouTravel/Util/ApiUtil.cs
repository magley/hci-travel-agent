using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json;

// https://learn.microsoft.com/en-us/bingmaps/rest-services/locations/location-recognition
namespace YouTravel.Util.Api
{
    internal class Address
    {
        public string? AddressLine { get; set; }
    }

    internal class Resource
    {
        public IList<Address>? AddressOfLocation { get; set; }
    }

    internal class ResourceSet
    {
        public IList<Resource>? Resources { get; set; }
    }

    internal class ApiResponse
    {
        public IList<ResourceSet>? ResourceSets { get; set; }
    }

    public static class LocationRecognition
    {

        private const string urlPath = "https://dev.virtualearth.net/REST/v1/LocationRecog";
        private static readonly string apiKey = File.ReadAllText("Data/MapsApiKey.apikey");
        private static readonly HttpClient client = new();
        private static readonly JsonSerializerOptions options = new()
        {
            PropertyNameCaseInsensitive = true
        };

        private static string Point(double lat, double lng)
        {
            return $"{lat.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture)},{lng.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture)}";
        }
        private static string Url(string point, string key)
        {
            return $"{urlPath}/{point}?key={key}&output=json&includeEntityTypes=address";
        }
        private static string SendRequest(double lat, double lng)
        {
            var url = Url(Point(lat, lng), apiKey);
#if DEBUG
            Console.WriteLine($"GET: {url}");
#endif
            var response = client.GetStringAsync(url).Result;
#if DEBUG
            Console.WriteLine($"RESPONSE: {response}");
#endif
            return response;
        }
        private static ApiResponse? FetchApiResponse(double lat, double lng)
        {
            var response = SendRequest(lat, lng);
            return JsonSerializer.Deserialize<ApiResponse>(response, options);
        }

        private delegate T NoArgumentDelegate<T>();
        public static string? FetchFirstLocationAddress(double lat, double lng)
        {
            static T? GetFirst<T>(NoArgumentDelegate<T> action)
            {
                T? item = default;
                try
                {
                    item = action.Invoke();
                }
                catch (ArgumentOutOfRangeException) { }
                return item;
            }

            var response = FetchApiResponse(lat, lng);

            Resource? resource = GetFirst(() => response?.ResourceSets?[0].Resources?[0]);
            if (resource == null)
            {
                return null;
            }

            string? addressLine = GetFirst(() => resource.AddressOfLocation?[0].AddressLine);
            if (addressLine != null)
            {
                return addressLine;
            }

            return null;
        }
    }
}
