using System.Text.Json.Serialization;

namespace AddressValidationPoc.Models;

// ── Our API Request Models ──────────────────────────────────────────────────

public record TextSearchRequest(
    string TextQuery,
    string? LanguageCode = "en",
    string? RegionCode = null,
    int MaxResultCount = 10,
    double? BiasLatitude = null,
    double? BiasLongitude = null,
    double BiasRadiusMeters = 5000.0
);

public record NearbySearchRequest(
    double Latitude,
    double Longitude,
    double RadiusMeters = 500.0,
    string[]? IncludedTypes = null,
    string[]? ExcludedTypes = null,
    string[]? IncludedPrimaryTypes = null,
    int MaxResultCount = 10,
    string? LanguageCode = "en",
    string? RegionCode = null
);

public record PlaceDetailsRequest(
    string PlaceId,
    string? LanguageCode = "en",
    string? RegionCode = null,
    string? SessionToken = null
);

// ── Google API Request Models ───────────────────────────────────────────────

public record GoogleTextSearchRequest(
    [property: JsonPropertyName("textQuery")] string TextQuery,
    [property: JsonPropertyName("languageCode")] string? LanguageCode,
    [property: JsonPropertyName("regionCode")] string? RegionCode,
    [property: JsonPropertyName("maxResultCount")] int MaxResultCount,
    [property: JsonPropertyName("locationBias")] GoogleLocationBias? LocationBias
);

public record GoogleNearbySearchRequest(
    [property: JsonPropertyName("locationRestriction")] GoogleLocationRestriction LocationRestriction,
    [property: JsonPropertyName("includedTypes")] string[]? IncludedTypes,
    [property: JsonPropertyName("excludedTypes")] string[]? ExcludedTypes,
    [property: JsonPropertyName("includedPrimaryTypes")] string[]? IncludedPrimaryTypes,
    [property: JsonPropertyName("maxResultCount")] int MaxResultCount,
    [property: JsonPropertyName("languageCode")] string? LanguageCode,
    [property: JsonPropertyName("regionCode")] string? RegionCode
);

public record GoogleLocationBias(
    [property: JsonPropertyName("circle")] GooglePlaceCircle Circle
);

public record GoogleLocationRestriction(
    [property: JsonPropertyName("circle")] GooglePlaceCircle Circle
);

public record GooglePlaceCircle(
    [property: JsonPropertyName("center")] GooglePlaceLatLng Center,
    [property: JsonPropertyName("radius")] double Radius
);

public record GooglePlaceLatLng(
    [property: JsonPropertyName("latitude")] double Latitude,
    [property: JsonPropertyName("longitude")] double Longitude
);

// ── Google API Response Models ──────────────────────────────────────────────

public record PlacesSearchResponse(
    [property: JsonPropertyName("places")] Place[]? Places,
    [property: JsonPropertyName("nextPageToken")] string? NextPageToken
);

public record Place(
    [property: JsonPropertyName("id")] string? Id,
    [property: JsonPropertyName("displayName")] LocalizedText? DisplayName,
    [property: JsonPropertyName("formattedAddress")] string? FormattedAddress,
    [property: JsonPropertyName("shortFormattedAddress")] string? ShortFormattedAddress,
    [property: JsonPropertyName("location")] GooglePlaceLatLng? Location,
    [property: JsonPropertyName("types")] string[]? Types,
    [property: JsonPropertyName("primaryType")] string? PrimaryType,
    [property: JsonPropertyName("primaryTypeDisplayName")] LocalizedText? PrimaryTypeDisplayName,
    [property: JsonPropertyName("rating")] double? Rating,
    [property: JsonPropertyName("userRatingCount")] int? UserRatingCount,
    [property: JsonPropertyName("priceLevel")] string? PriceLevel,
    [property: JsonPropertyName("businessStatus")] string? BusinessStatus,
    [property: JsonPropertyName("regularOpeningHours")] PlaceOpeningHours? RegularOpeningHours,
    [property: JsonPropertyName("currentOpeningHours")] PlaceOpeningHours? CurrentOpeningHours,
    [property: JsonPropertyName("nationalPhoneNumber")] string? NationalPhoneNumber,
    [property: JsonPropertyName("internationalPhoneNumber")] string? InternationalPhoneNumber,
    [property: JsonPropertyName("googleMapsUri")] string? GoogleMapsUri,
    [property: JsonPropertyName("websiteUri")] string? WebsiteUri,
    [property: JsonPropertyName("editorialSummary")] LocalizedText? EditorialSummary,
    [property: JsonPropertyName("plusCode")] PlacePlusCode? PlusCode,
    [property: JsonPropertyName("viewport")] PlaceViewport? Viewport,
    [property: JsonPropertyName("addressComponents")] PlaceAddressComponent[]? AddressComponents,
    [property: JsonPropertyName("adrFormatAddress")] string? AdrFormatAddress,
    [property: JsonPropertyName("utcOffsetMinutes")] int? UtcOffsetMinutes,
    [property: JsonPropertyName("iconMaskBaseUri")] string? IconMaskBaseUri,
    [property: JsonPropertyName("iconBackgroundColor")] string? IconBackgroundColor
);

public record PlaceAddressComponent(
    [property: JsonPropertyName("longText")] string? LongText,
    [property: JsonPropertyName("shortText")] string? ShortText,
    [property: JsonPropertyName("types")] string[]? Types,
    [property: JsonPropertyName("languageCode")] string? LanguageCode
);

public record LocalizedText(
    [property: JsonPropertyName("text")] string? Text,
    [property: JsonPropertyName("languageCode")] string? LanguageCode
);

public record PlaceOpeningHours(
    [property: JsonPropertyName("openNow")] bool? OpenNow,
    [property: JsonPropertyName("periods")] OpeningHoursPeriod[]? Periods,
    [property: JsonPropertyName("weekdayDescriptions")] string[]? WeekdayDescriptions
);

public record OpeningHoursPeriod(
    [property: JsonPropertyName("open")] OpeningHoursPoint? Open,
    [property: JsonPropertyName("close")] OpeningHoursPoint? Close
);

public record OpeningHoursPoint(
    [property: JsonPropertyName("day")] int? Day,
    [property: JsonPropertyName("hour")] int? Hour,
    [property: JsonPropertyName("minute")] int? Minute
);

public record PlacePlusCode(
    [property: JsonPropertyName("globalCode")] string? GlobalCode,
    [property: JsonPropertyName("compoundCode")] string? CompoundCode
);

public record PlaceViewport(
    [property: JsonPropertyName("low")] GooglePlaceLatLng? Low,
    [property: JsonPropertyName("high")] GooglePlaceLatLng? High
);
