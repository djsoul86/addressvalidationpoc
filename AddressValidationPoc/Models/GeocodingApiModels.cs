using System.Text.Json.Serialization;

namespace AddressValidationPoc.Models;

// ── Our API Request Model ───────────────────────────────────────────────────

public record GeocodeRequest(
    string Address,
    string? Region = null,
    string? Language = null,
    string? Components = null
);

// ── Google Geocoding API Response Models ────────────────────────────────────

public record GoogleGeocodeResponse(
    [property: JsonPropertyName("results")] GeocodeResult[]? Results,
    [property: JsonPropertyName("status")] string? Status,
    [property: JsonPropertyName("error_message")] string? ErrorMessage
);

public record GeocodeResult(
    [property: JsonPropertyName("address_components")] GeocodeAddressComponent[]? AddressComponents,
    [property: JsonPropertyName("formatted_address")] string? FormattedAddress,
    [property: JsonPropertyName("geometry")] GeocodeGeometry? Geometry,
    [property: JsonPropertyName("place_id")] string? PlaceId,
    [property: JsonPropertyName("types")] string[]? Types,
    [property: JsonPropertyName("partial_match")] bool? PartialMatch
);

public record GeocodeAddressComponent(
    [property: JsonPropertyName("long_name")] string? LongName,
    [property: JsonPropertyName("short_name")] string? ShortName,
    [property: JsonPropertyName("types")] string[]? Types
);

public record GeocodeGeometry(
    [property: JsonPropertyName("location")] GeocodeLatLng? Location,
    [property: JsonPropertyName("location_type")] string? LocationType,
    [property: JsonPropertyName("viewport")] GeocodeBounds? Viewport,
    [property: JsonPropertyName("bounds")] GeocodeBounds? Bounds
);

public record GeocodeLatLng(
    [property: JsonPropertyName("lat")] double Lat,
    [property: JsonPropertyName("lng")] double Lng
);

public record GeocodeBounds(
    [property: JsonPropertyName("northeast")] GeocodeLatLng? Northeast,
    [property: JsonPropertyName("southwest")] GeocodeLatLng? Southwest
);
