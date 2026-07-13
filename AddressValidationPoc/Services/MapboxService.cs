using System.Net.Http.Json;
using AddressValidationPoc.Models;
using Microsoft.AspNetCore.WebUtilities;

namespace AddressValidationPoc.Services;

public interface IMapboxService
{
    Task<MapboxSuggestResponse> SuggestAsync(MapboxSuggestRequest request, CancellationToken ct = default);
    Task<MapboxSearchBoxFeatureCollection> RetrieveAsync(MapboxRetrieveRequest request, CancellationToken ct = default);
    Task<MapboxSearchBoxFeatureCollection> ForwardSearchAsync(MapboxForwardSearchRequest request, CancellationToken ct = default);
}

public class MapboxService(HttpClient httpClient, IConfiguration configuration) : IMapboxService
{
    private const string BaseUrl = "https://api.mapbox.com/search/searchbox/v1";

    private string AccessToken =>
        configuration["Mapbox:AccessToken"]
        ?? throw new InvalidOperationException("Mapbox access token is not configured.");

    public async Task<MapboxSuggestResponse> SuggestAsync(MapboxSuggestRequest request, CancellationToken ct = default)
    {
        var queryParams = new Dictionary<string, string?>
        {
            ["q"] = request.Query,
        };

        return await GetAsync<MapboxSuggestResponse>($"{BaseUrl}/suggest", queryParams, ct);
    }

    public async Task<MapboxSearchBoxFeatureCollection> RetrieveAsync(MapboxRetrieveRequest request, CancellationToken ct = default)
    {
        var queryParams = new Dictionary<string, string?>();

        return await GetAsync<MapboxSearchBoxFeatureCollection>(
            $"{BaseUrl}/retrieve/{Uri.EscapeDataString(request.MapboxId)}", queryParams, ct);
    }

    public async Task<MapboxSearchBoxFeatureCollection> ForwardSearchAsync(MapboxForwardSearchRequest request, CancellationToken ct = default)
    {
        var queryParams = new Dictionary<string, string?>
        {
            ["q"] = request.Query,
        };

        return await GetAsync<MapboxSearchBoxFeatureCollection>($"{BaseUrl}/forward", queryParams, ct);
    }

    private async Task<T> GetAsync<T>(string url, Dictionary<string, string?> queryParams, CancellationToken ct)
    {
        queryParams["access_token"] = AccessToken;
        queryParams["session_token"] = Guid.NewGuid().ToString();
        var fullUrl = QueryHelpers.AddQueryString(url, queryParams);

        using var message = new HttpRequestMessage(HttpMethod.Get, fullUrl);

        var response = await httpClient.SendAsync(message, ct);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<T>(ct)
            ?? throw new InvalidOperationException("Empty response from Mapbox Search Box API.");
    }
}
