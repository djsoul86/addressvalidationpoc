using System.Text.Json.Serialization;

namespace AddressValidationPoc.Models;

// ── Google API Request ──────────────────────────────────────────────────────

public record GoogleValidateRequest(
    [property: JsonPropertyName("address")] GooglePostalAddress Address,
    [property: JsonPropertyName("enableUspsCass")] bool EnableUspsCass = true
);

public record GooglePostalAddress(
    [property: JsonPropertyName("addressLines")] string[] AddressLines,
    [property: JsonPropertyName("regionCode")] string? RegionCode = null,
    [property: JsonPropertyName("locality")] string? Locality = null,
    [property: JsonPropertyName("administrativeArea")] string? AdministrativeArea = null,
    [property: JsonPropertyName("postalCode")] string? PostalCode = null
);

// ── Google API Response ─────────────────────────────────────────────────────

public record GoogleValidateResponse(
    [property: JsonPropertyName("result")] ValidationResult? Result,
    [property: JsonPropertyName("responseId")] string? ResponseId
);

public record ValidationResult(
    [property: JsonPropertyName("verdict")] Verdict? Verdict,
    [property: JsonPropertyName("address")] ValidatedAddress? Address,
    [property: JsonPropertyName("geocode")] Geocode? Geocode,
    [property: JsonPropertyName("metadata")] AddressMetadata? Metadata,
    [property: JsonPropertyName("uspsData")] UspsData? UspsData
);

public record Verdict(
    [property: JsonPropertyName("inputGranularity")] string? InputGranularity,
    [property: JsonPropertyName("validationGranularity")] string? ValidationGranularity,
    [property: JsonPropertyName("geocodeGranularity")] string? GeocodeGranularity,
    [property: JsonPropertyName("addressComplete")] bool AddressComplete,
    [property: JsonPropertyName("hasUnconfirmedComponents")] bool HasUnconfirmedComponents,
    [property: JsonPropertyName("hasInferredComponents")] bool HasInferredComponents,
    [property: JsonPropertyName("hasReplacedComponents")] bool HasReplacedComponents
);

public record ValidatedAddress(
    [property: JsonPropertyName("formattedAddress")] string? FormattedAddress,
    [property: JsonPropertyName("postalAddress")] GooglePostalAddress? PostalAddress,
    [property: JsonPropertyName("addressComponents")] AddressComponent[]? AddressComponents,
    [property: JsonPropertyName("missingComponentTypes")] string[]? MissingComponentTypes,
    [property: JsonPropertyName("unconfirmedComponentTypes")] string[]? UnconfirmedComponentTypes,
    [property: JsonPropertyName("unresolvedTokens")] string[]? UnresolvedTokens
);

public record AddressComponent(
    [property: JsonPropertyName("componentName")] ComponentName? ComponentName,
    [property: JsonPropertyName("componentType")] string? ComponentType,
    [property: JsonPropertyName("confirmationLevel")] string? ConfirmationLevel,
    [property: JsonPropertyName("inferred")] bool Inferred,
    [property: JsonPropertyName("spellCorrected")] bool SpellCorrected,
    [property: JsonPropertyName("replaced")] bool Replaced,
    [property: JsonPropertyName("unexpected")] bool Unexpected
);

public record ComponentName(
    [property: JsonPropertyName("text")] string? Text,
    [property: JsonPropertyName("languageCode")] string? LanguageCode
);

public record Geocode(
    [property: JsonPropertyName("location")] LatLng? Location,
    [property: JsonPropertyName("plusCode")] PlusCode? PlusCode,
    [property: JsonPropertyName("placeId")] string? PlaceId,
    [property: JsonPropertyName("placeTypes")] string[]? PlaceTypes
);

public record LatLng(
    [property: JsonPropertyName("latitude")] double Latitude,
    [property: JsonPropertyName("longitude")] double Longitude
);

public record PlusCode(
    [property: JsonPropertyName("globalCode")] string? GlobalCode,
    [property: JsonPropertyName("compoundCode")] string? CompoundCode
);

public record AddressMetadata(
    [property: JsonPropertyName("business")] bool Business,
    [property: JsonPropertyName("poBox")] bool PoBox,
    [property: JsonPropertyName("residential")] bool Residential
);

public record UspsData(
    [property: JsonPropertyName("standardizedAddress")] UspsAddress? StandardizedAddress,
    [property: JsonPropertyName("deliveryPointCode")] string? DeliveryPointCode,
    [property: JsonPropertyName("deliveryPointCheckDigit")] string? DeliveryPointCheckDigit,
    [property: JsonPropertyName("dpvConfirmation")] string? DpvConfirmation,
    [property: JsonPropertyName("dpvFootnote")] string? DpvFootnote,
    [property: JsonPropertyName("dpvCmra")] string? DpvCmra,
    [property: JsonPropertyName("dpvVacant")] string? DpvVacant,
    [property: JsonPropertyName("dpvNoStat")] string? DpvNoStat,
    [property: JsonPropertyName("carrierRoute")] string? CarrierRoute,
    [property: JsonPropertyName("carrierRouteIndicator")] string? CarrierRouteIndicator,
    [property: JsonPropertyName("postOfficeCity")] string? PostOfficeCity,
    [property: JsonPropertyName("postOfficeState")] string? PostOfficeState,
    [property: JsonPropertyName("abbreviatedCity")] string? AbbreviatedCity,
    [property: JsonPropertyName("fipsCountyCode")] string? FipsCountyCode,
    [property: JsonPropertyName("county")] string? County,
    [property: JsonPropertyName("elotNumber")] string? ElotNumber,
    [property: JsonPropertyName("elotFlag")] string? ElotFlag,
    [property: JsonPropertyName("poBoxOnlyPostalCode")] bool PoBoxOnlyPostalCode,
    [property: JsonPropertyName("suitelinkFootnote")] string? SuitelinkFootnote,
    [property: JsonPropertyName("pmbDesignator")] string? PmbDesignator,
    [property: JsonPropertyName("pmbNumber")] string? PmbNumber,
    [property: JsonPropertyName("addressRecordType")] string? AddressRecordType,
    [property: JsonPropertyName("defaultAddress")] bool DefaultAddress,
    [property: JsonPropertyName("errorMessage")] string? ErrorMessage,
    [property: JsonPropertyName("cassProcessed")] bool CassProcessed
);

public record UspsAddress(
    [property: JsonPropertyName("firstAddressLine")] string? FirstAddressLine,
    [property: JsonPropertyName("firm")] string? Firm,
    [property: JsonPropertyName("secondAddressLine")] string? SecondAddressLine,
    [property: JsonPropertyName("urbanization")] string? Urbanization,
    [property: JsonPropertyName("cityStateZipAddressLine")] string? CityStateZipAddressLine,
    [property: JsonPropertyName("city")] string? City,
    [property: JsonPropertyName("state")] string? State,
    [property: JsonPropertyName("zipCode")] string? ZipCode,
    [property: JsonPropertyName("zipCodeExtension")] string? ZipCodeExtension
);
