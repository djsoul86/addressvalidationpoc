using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using AddressValidationPoc.Models;

namespace AddressValidationPoc.Services;

public interface IPlacesService
{
    Task<PlacesSearchResponse> TextSearchAsync(TextSearchRequest request, CancellationToken ct = default);
    Task<PlacesSearchResponse> NearbySearchAsync(NearbySearchRequest request, CancellationToken ct = default);
    Task<Place> GetPlaceDetailsAsync(PlaceDetailsRequest request, CancellationToken ct = default);
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

    private const string DetailsFieldMask =
        "id,displayName,formattedAddress,shortFormattedAddress,addressComponents,adrFormatAddress," +
        "location,types,primaryType,primaryTypeDisplayName,rating,userRatingCount,priceLevel," +
        "businessStatus,regularOpeningHours,currentOpeningHours,nationalPhoneNumber," +
        "internationalPhoneNumber,googleMapsUri,websiteUri,editorialSummary,plusCode,viewport," +
        "utcOffsetMinutes,iconMaskBaseUri,iconBackgroundColor";

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

    public async Task<Place> GetPlaceDetailsAsync(PlaceDetailsRequest request, CancellationToken ct = default)
    {
        var query = new List<string>();
        if (!string.IsNullOrWhiteSpace(request.LanguageCode))
            query.Add($"languageCode={Uri.EscapeDataString(request.LanguageCode)}");
        if (!string.IsNullOrWhiteSpace(request.RegionCode))
            query.Add($"regionCode={Uri.EscapeDataString(request.RegionCode)}");
        if (!string.IsNullOrWhiteSpace(request.SessionToken))
            query.Add($"sessionToken={Uri.EscapeDataString(request.SessionToken)}");

        var queryString = query.Count > 0 ? $"?{string.Join("&", query)}" : string.Empty;

        var placeId = Uri.EscapeDataString(request.PlaceId);
        using var message = new HttpRequestMessage(HttpMethod.Get, $"{BaseUrl}/{placeId}{queryString}");
        message.Headers.Add("X-Goog-Api-Key", ApiKey);
        message.Headers.Add("X-Goog-FieldMask", DetailsFieldMask);

        var response = await httpClient.SendAsync(message, ct);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<Place>(ct)
            ?? throw new InvalidOperationException("Empty response from Google Places API.");
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
