using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json;

namespace YouTravel.Util.Api
{
    internal class BusinessInfo
    {
        public string? EntityName { get; set; }
    }

    internal class Business
    {
        public BusinessInfo? BusinessInfo { get; set; }
    }

    internal class POI
    {
        public string? EntityName { get; set; }
    }

    internal class Resource
    {
        public IList<POI>? NaturalPOIAtLocation { get; set; }
        public IList<Business>? BusinessesAtLocation { get; set; }
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
            return $"{lat},{lng}";
        }
        private static string Url(string point, string key)
        {
            return $"{urlPath}/{point}?key={key}&output=json";
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
        public static string? FetchFirstLocationName(double lat, double lng)
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

            string? naturalPOIName = GetFirst(() => resource.NaturalPOIAtLocation?[0].EntityName);
            if (naturalPOIName != null)
            {
                return naturalPOIName;
            }

            string? businessName = GetFirst(() => resource.BusinessesAtLocation?[0].BusinessInfo?.EntityName);
            if (businessName != null)
            {
                return businessName;
            }

            return null;
        }
    }
}
