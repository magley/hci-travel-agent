using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

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

    public class NoMapsAPIException : Exception
    {

    }

    public static class MapsAPI
    {
		public static readonly string Key = "";

		static MapsAPI()
		{
			try
			{
				Key = File.ReadAllText("Data/MapsApiKey.apikey");
			}
			catch (FileNotFoundException)
			{
				Key = "";
			}
		}
	}

    public static class LocationRecognition
    {
        private const string urlPath = "https://dev.virtualearth.net/REST/v1/LocationRecog";
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
        private static async Task<string> SendRequestAsync(double lat, double lng)
        {
            var url = Url(Point(lat, lng), MapsAPI.Key);
            try
            {
                return await client.GetStringAsync(url);
            }
            catch (System.Net.Http.HttpRequestException)
            {
                return "{}";
            }
        }
        private static async Task<ApiResponse?> FetchApiResponse(double lat, double lng)
        {
            var response = await SendRequestAsync(lat, lng);
            return JsonSerializer.Deserialize<ApiResponse>(response, options);
        }

        private delegate T NoArgumentDelegate<T>();
        public static async Task<string?> FetchFirstLocationAddressAsync(double lat, double lng)
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

            var response = await FetchApiResponse(lat, lng);

            Resource? resource = GetFirst(() => response?.ResourceSets?[0].Resources?[0]);
            if (resource == null)
            {
                return null;
            }

            string? addressLine = GetFirst(() => resource.AddressOfLocation?[0].AddressLine);
            if (addressLine != null && addressLine != "")
            {
                return addressLine;
            }

            return null;
        }
    }
}
