using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.WebUtilities;

namespace MotoHealth.Functions
{
    public interface IGoogleMapsService
    {
        Uri GetLocationPinUri(double latitude, double longitude);
    }

    internal sealed class GoogleMapsService : IGoogleMapsService
    {
        private const string GoogleMapsSearchBasePath = "https://www.google.com/maps/search/";

        public Uri GetLocationPinUri(double latitude, double longitude)
        {
            var queryParams = new Dictionary<string, string>
            {
                { "api", "1" },
                { "query", FormattableString.Invariant($"{latitude},{longitude}") },
            };

            var uriString = QueryHelpers.AddQueryString(GoogleMapsSearchBasePath, queryParams);

            return new Uri(uriString, UriKind.Absolute);
        }
    }
}