using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using AddressValidationPoc.Models;

namespace AddressValidationPoc.Services;

public interface IPlacesService
{
    Task<PlacesSearchResponse> TextSearchAsync(TextSearchRequest request, CancellationToken ct = default);
    Task<PlacesSearchResponse> NearbySearchAsync(NearbySearchRequest request, CancellationToken ct = default);
}

public class PlacesService(HttpClient httpClient, IConfiguration configuration) : IPlacesService
{
    private const string BaseUrl = "https://places.googleapis.com/v1/places";

    private const string DefaultFieldMask =
        "places.id,places.displayName,places.formattedAddress,places.shortFormattedAddress," +
        "places.location,places.types,places.primaryType,places.primaryTypeDisplayName," +
        "places.rating,places.userRatingCount,places.priceLevel,places.businessStatus," +
        "places.regularOpeningHours,places.nationalPhoneNumber,places.internationalPhoneNumber," +
        "places.googleMapsUri,places.websiteUri,places.editorialSummary,places.plusCode," +
        "nextPageToken";

    private string ApiKey =>
        configuration["Google:Places:ApiKey"]
        ?? configuration["Google:AddressValidation:ApiKey"]
        ?? throw new InvalidOperationException("Google Places API key is not configured.");

    public async Task<PlacesSearchResponse> TextSearchAsync(TextSearchRequest request, CancellationToken ct = default)
    {
        var googleRequest = new GoogleTextSearchRequest(
            TextQuery: request.TextQuery,
            LanguageCode: request.LanguageCode,
            RegionCode: request.RegionCode,
            MaxResultCount: request.MaxResultCount,
            LocationBias: request.BiasLatitude is null || request.BiasLongitude is null
                ? null
                : new GoogleLocationBias(
                    Circle: new GooglePlaceCircle(
                        Center: new GooglePlaceLatLng(request.BiasLatitude.Value, request.BiasLongitude.Value),
                        Radius: request.BiasRadiusMeters
                    )
                )
        );

        return await PostAsync(":searchText", googleRequest, DefaultFieldMask, ct);
    }

    public async Task<PlacesSearchResponse> NearbySearchAsync(NearbySearchRequest request, CancellationToken ct = default)
    {
        var googleRequest = new GoogleNearbySearchRequest(
            LocationRestriction: new GoogleLocationRestriction(
                Circle: new GooglePlaceCircle(
                    Center: new GooglePlaceLatLng(request.Latitude, request.Longitude),
                    Radius: request.RadiusMeters
                )
            ),
            IncludedTypes: request.IncludedTypes,
            ExcludedTypes: request.ExcludedTypes,
            IncludedPrimaryTypes: request.IncludedPrimaryTypes,
            MaxResultCount: request.MaxResultCount,
            LanguageCode: request.LanguageCode,
            RegionCode: request.RegionCode
        );

        return await PostAsync(":searchNearby", googleRequest, DefaultFieldMask, ct);
    }

    private async Task<PlacesSearchResponse> PostAsync<TRequest>(
        string path, TRequest payload, string fieldMask, CancellationToken ct)
    {
        var json = JsonSerializer.Serialize(payload, new JsonSerializerOptions
        {
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        });

        using var message = new HttpRequestMessage(HttpMethod.Post, $"{BaseUrl}{path}")
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };
        message.Headers.Add("X-Goog-Api-Key", ApiKey);
        message.Headers.Add("X-Goog-FieldMask", fieldMask);

        var response = await httpClient.SendAsync(message, ct);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<PlacesSearchResponse>(ct)
            ?? throw new InvalidOperationException("Empty response from Google Places API.");
    }
}
