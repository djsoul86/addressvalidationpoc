using AddressValidationPoc.Models;
using AddressValidationPoc.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddHttpClient<IAddressValidationService, AddressValidationService>();
builder.Services.AddHttpClient<IPlacesService, PlacesService>();
builder.Services.AddHttpClient<IMapboxService, MapboxService>();
builder.Services.AddHttpClient<IGeocodingService, GeocodingService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// ── Google Address Validation ───────────────────────────────────────────────

app.MapPost("/api/validate-address", async (
    ValidateAddressRequest request,
    IAddressValidationService service,
    CancellationToken ct) =>
{
    var result = await service.ValidateAsync(request, ct);
    return Results.Ok(result);
})
.WithName("ValidateAddress")
.WithSummary("Validate a postal address using the Google Address Validation API")
.WithDescription("""
    Submits an address to the Google Address Validation API and returns the full result,
    including verdict, corrected address components, geocode, metadata, and USPS data.
    """);

// ── Google Geocoding API ────────────────────────────────────────────────────

app.MapGet("/api/geocode", async (
    string address,
    string? region,
    string? language,
    string? components,
    IGeocodingService service,
    CancellationToken ct) =>
{
    var request = new GeocodeRequest(address, region, language, components);
    var result = await service.GeocodeAsync(request, ct);
    return Results.Ok(result);
})
.WithName("Geocode")
.WithSummary("Geocode an address using the Google Geocoding API")
.WithDescription("""
    Converts a free-form address into geographic coordinates (lat/lng), formatted address,
    address components, place ID, location type, and viewport/bounds.
    Optional region (ccTLD bias, e.g. "ca"), language, and components (filter, e.g.
    "country:US|postal_code:90210") parameters narrow or bias the results.
    """);

// ── Google Places API (New) ─────────────────────────────────────────────────

app.MapPost("/api/places/text-search", async (
    TextSearchRequest request,
    IPlacesService service,
    CancellationToken ct) =>
{
    var result = await service.TextSearchAsync(request, ct);
    return Results.Ok(result);
})
.WithName("PlacesTextSearch")
.WithSummary("Search places by text using the Google Places API (New)")
.WithDescription("""
    Performs a full-text search for places matching the query. Optionally biases results
    toward a geographic area (circle defined by lat/lng + radius in meters).
    Returns display name, address, location, types, rating, opening hours, and more.
    """);

app.MapPost("/api/places/nearby-search", async (
    NearbySearchRequest request,
    IPlacesService service,
    CancellationToken ct) =>
{
    var result = await service.NearbySearchAsync(request, ct);
    return Results.Ok(result);
})
.WithName("PlacesNearbySearch")
.WithSummary("Search places near a location using the Google Places API (New)")
.WithDescription("""
    Returns places within a circular area defined by center (lat/lng) and radius (meters).
    Optionally filter by place types (e.g. restaurant, cafe, hospital).
    """);

app.MapGet("/api/places/{placeId}", async (
    string placeId,
    string? languageCode,
    string? regionCode,
    string? sessionToken,
    IPlacesService service,
    CancellationToken ct) =>
{
    var request = new PlaceDetailsRequest(placeId, languageCode ?? "en", regionCode, sessionToken);
    var result = await service.GetPlaceDetailsAsync(request, ct);
    return Results.Ok(result);
})
.WithName("PlaceDetails")
.WithSummary("Get full details for a place using the Google Places API (New)")
.WithDescription("""
    Fetches complete details for a place given its place ID (as returned by text-search,
    nearby-search, or Autocomplete). Returns address components, formatted address, location,
    opening hours, contact info, and more.
    If the place ID came from an Autocomplete session, pass the same sessionToken to group billing.
    """);

// ── Mapbox Search Box API ───────────────────────────────────────────────────

app.MapPost("/api/mapbox/suggest", async (
    MapboxSuggestRequest request,
    IMapboxService service,
    CancellationToken ct) =>
{
    var result = await service.SuggestAsync(request, ct);
    return Results.Ok(result);
})
.WithName("MapboxSuggest")
.WithSummary("Get address/place suggestions using Mapbox Search Box API")
.WithDescription("""
    Returns lightweight autocomplete suggestions for the given query.
    Copy the mapbox_id from a suggestion and pass it to /api/mapbox/retrieve
    to get full coordinates and details.
    Use the same session_token (UUID) across suggest + retrieve calls to group billing.
    """);

app.MapPost("/api/mapbox/retrieve", async (
    MapboxRetrieveRequest request,
    IMapboxService service,
    CancellationToken ct) =>
{
    var result = await service.RetrieveAsync(request, ct);
    return Results.Ok(result);
})
.WithName("MapboxRetrieve")
.WithSummary("Retrieve full place details for a Mapbox Search Box suggestion")
.WithDescription("""
    Fetches complete place details (coordinates, accuracy, routable_points, full context)
    for a mapbox_id returned by /api/mapbox/suggest.
    Must use the same session_token as the preceding suggest call.
    """);

app.MapPost("/api/mapbox/forward-search", async (
    MapboxForwardSearchRequest request,
    IMapboxService service,
    CancellationToken ct) =>
{
    var result = await service.ForwardSearchAsync(request, ct);
    return Results.Ok(result);
})
.WithName("MapboxForwardSearch")
.WithSummary("Single-step place search using Mapbox Search Box API")
.WithDescription("""
    Combines suggest + retrieve in one call. Returns a GeoJSON FeatureCollection
    with full place details including coordinates and accuracy.
    Ideal for server-to-server use where the two-step flow is not needed.
    """);

app.Run();
