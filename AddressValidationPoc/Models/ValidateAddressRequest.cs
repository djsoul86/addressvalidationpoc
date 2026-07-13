namespace AddressValidationPoc.Models;

public record ValidateAddressRequest(
    string[] AddressLines,
    string RegionCode = "US",
    string? Locality = null,
    string? AdministrativeArea = null,
    string? PostalCode = null
);
