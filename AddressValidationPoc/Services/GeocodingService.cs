using System.Net.Http.Json;
using AddressValidationPoc.Models;

namespace AddressValidationPoc.Services;

public interface IGeocodingService
{
    Task<GoogleGeocodeResponse> GeocodeAsync(GeocodeRequest request, CancellationToken ct = default);
}

public class GeocodingService(HttpClient httpClient, IConfiguration configuration) : IGeocodingService
{
    private const string BaseUrl = "https://maps.googleapis.com/maps/api/geocode/json";

    private string ApiKey =>
        configuration["Google:Geocoding:ApiKey"]
        ?? configuration["Google:AddressValidation:ApiKey"]
        ?? throw new InvalidOperationException("Google Geocoding API key is not configured.");

    public async Task<GoogleGeocodeResponse> GeocodeAsync(GeocodeRequest request, CancellationToken ct = default)
    {
        var query = new List<string> { $"address={Uri.EscapeDataString(request.Address)}", $"key={ApiKey}" };

        if (!string.IsNullOrWhiteSpace(request.Region))
            query.Add($"region={Uri.EscapeDataString(request.Region)}");
        if (!string.IsNullOrWhiteSpace(request.Language))
            query.Add($"language={Uri.EscapeDataString(request.Language)}");
        if (!string.IsNullOrWhiteSpace(request.Components))
            query.Add($"components={Uri.EscapeDataString(request.Components)}");

        var response = await httpClient.GetAsync($"{BaseUrl}?{string.Join("&", query)}", ct);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<GoogleGeocodeResponse>(ct)
            ?? throw new InvalidOperationException("Empty response from Google Geocoding API.");
    }
}
