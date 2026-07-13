using System.Text.Json.Serialization;

namespace AddressValidationPoc.Models;

// ── Our API Request Models ──────────────────────────────────────────────────

public record MapboxSuggestRequest(
    string Query,
    string SessionToken,
    string? Country = null,
    string Language = "en",
    int Limit = 5,
    double? ProximityLongitude = null,
    double? ProximityLatitude = null,
    string[]? Types = null
);

public record MapboxRetrieveRequest(
    string MapboxId,
    string SessionToken
);

public record MapboxForwardSearchRequest(
    string Query,
    string SessionToken,
    string? Country = null,
    string Language = "en",
    int Limit = 5,
    double? ProximityLongitude = null,
    double? ProximityLatitude = null
);

// ── Suggest Response ────────────────────────────────────────────────────────

public record MapboxSuggestResponse(
    [property: JsonPropertyName("suggestions")] MapboxSuggestion[]? Suggestions,
    [property: JsonPropertyName("attribution")] string? Attribution
);

public record MapboxSuggestion(
    [property: JsonPropertyName("name")] string? Name,
    [property: JsonPropertyName("mapbox_id")] string? MapboxId,
    [property: JsonPropertyName("feature_type")] string? FeatureType,
    [property: JsonPropertyName("place_formatted")] string? PlaceFormatted,
    [property: JsonPropertyName("full_address")] string? FullAddress,
    [property: JsonPropertyName("language")] string? Language,
    [property: JsonPropertyName("maki")] string? Maki,
    [property: JsonPropertyName("context")] MapboxSuggestContext? Context
);

public record MapboxSuggestContext(
    [property: JsonPropertyName("country")] MapboxSuggestContextCountry? Country,
    [property: JsonPropertyName("region")] MapboxSuggestContextRegion? Region,
    [property: JsonPropertyName("postcode")] MapboxSuggestContextEntry? Postcode,
    [property: JsonPropertyName("place")] MapboxSuggestContextEntry? Place,
    [property: JsonPropertyName("neighborhood")] MapboxSuggestContextEntry? Neighborhood,
    [property: JsonPropertyName("street")] MapboxSuggestContextEntry? Street
);

public record MapboxSuggestContextEntry(
    [property: JsonPropertyName("id")] string? Id,
    [property: JsonPropertyName("mapbox_id")] string? MapboxId,
    [property: JsonPropertyName("name")] string? Name
);

public record MapboxSuggestContextRegion(
    [property: JsonPropertyName("id")] string? Id,
    [property: JsonPropertyName("mapbox_id")] string? MapboxId,
    [property: JsonPropertyName("name")] string? Name,
    [property: JsonPropertyName("region_code")] string? RegionCode,
    [property: JsonPropertyName("region_code_full")] string? RegionCodeFull
);

public record MapboxSuggestContextCountry(
    [property: JsonPropertyName("id")] string? Id,
    [property: JsonPropertyName("mapbox_id")] string? MapboxId,
    [property: JsonPropertyName("name")] string? Name,
    [property: JsonPropertyName("country_code")] string? CountryCode,
    [property: JsonPropertyName("country_code_alpha_3")] string? CountryCodeAlpha3
);

// ── Retrieve / Forward Search Response (GeoJSON FeatureCollection) ──────────

public record MapboxSearchBoxFeatureCollection(
    [property: JsonPropertyName("type")] string? Type,
    [property: JsonPropertyName("features")] MapboxSearchBoxFeature[]? Features,
    [property: JsonPropertyName("attribution")] string? Attribution
);

public record MapboxSearchBoxFeature(
    [property: JsonPropertyName("type")] string? Type,
    [property: JsonPropertyName("geometry")] MapboxSearchBoxGeometry? Geometry,
    [property: JsonPropertyName("properties")] MapboxSearchBoxProperties? Properties
);

public record MapboxSearchBoxGeometry(
    [property: JsonPropertyName("type")] string? Type,
    // GeoJSON order: [longitude, latitude]
    [property: JsonPropertyName("coordinates")] double[]? Coordinates
);

public record MapboxSearchBoxProperties(
    [property: JsonPropertyName("mapbox_id")] string? MapboxId,
    [property: JsonPropertyName("feature_type")] string? FeatureType,
    [property: JsonPropertyName("name")] string? Name,
    [property: JsonPropertyName("name_preferred")] string? NamePreferred,
    [property: JsonPropertyName("place_formatted")] string? PlaceFormatted,
    [property: JsonPropertyName("full_address")] string? FullAddress,
    [property: JsonPropertyName("coordinates")] MapboxSearchBoxCoordinates? Coordinates,
    [property: JsonPropertyName("bbox")] double[]? Bbox,
    [property: JsonPropertyName("context")] MapboxSearchBoxContext? Context
);

public record MapboxSearchBoxCoordinates(
    [property: JsonPropertyName("longitude")] double Longitude,
    [property: JsonPropertyName("latitude")] double Latitude,
    [property: JsonPropertyName("accuracy")] string? Accuracy,
    [property: JsonPropertyName("routable_points")] MapboxRoutablePoint[]? RoutablePoints
);

public record MapboxRoutablePoint(
    [property: JsonPropertyName("name")] string? Name,
    [property: JsonPropertyName("latitude")] double Latitude,
    [property: JsonPropertyName("longitude")] double Longitude
);

public record MapboxSearchBoxContext(
    [property: JsonPropertyName("address")] MapboxSearchBoxContextAddress? Address,
    [property: JsonPropertyName("street")] MapboxSearchBoxContextEntry? Street,
    [property: JsonPropertyName("neighborhood")] MapboxSearchBoxContextEntry? Neighborhood,
    [property: JsonPropertyName("postcode")] MapboxSearchBoxContextEntry? Postcode,
    [property: JsonPropertyName("place")] MapboxSearchBoxContextEntry? Place,
    [property: JsonPropertyName("district")] MapboxSearchBoxContextEntry? District,
    [property: JsonPropertyName("region")] MapboxSearchBoxContextRegion? Region,
    [property: JsonPropertyName("country")] MapboxSearchBoxContextCountry? Country
);

public record MapboxSearchBoxContextAddress(
    [property: JsonPropertyName("mapbox_id")] string? MapboxId,
    [property: JsonPropertyName("name")] string? Name,
    [property: JsonPropertyName("address_number")] string? AddressNumber,
    [property: JsonPropertyName("street_name")] string? StreetName
);

public record MapboxSearchBoxContextEntry(
    [property: JsonPropertyName("mapbox_id")] string? MapboxId,
    [property: JsonPropertyName("name")] string? Name
);

public record MapboxSearchBoxContextRegion(
    [property: JsonPropertyName("mapbox_id")] string? MapboxId,
    [property: JsonPropertyName("name")] string? Name,
    [property: JsonPropertyName("region_code")] string? RegionCode,
    [property: JsonPropertyName("region_code_full")] string? RegionCodeFull
);

public record MapboxSearchBoxContextCountry(
    [property: JsonPropertyName("mapbox_id")] string? MapboxId,
    [property: JsonPropertyName("name")] string? Name,
    [property: JsonPropertyName("country_code")] string? CountryCode,
    [property: JsonPropertyName("country_code_alpha_3")] string? CountryCodeAlpha3
);
