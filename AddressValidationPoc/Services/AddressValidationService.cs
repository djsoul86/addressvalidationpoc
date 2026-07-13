using System.Net.Http.Json;
using AddressValidationPoc.Models;

namespace AddressValidationPoc.Services;

public interface IAddressValidationService
{
    Task<GoogleValidateResponse> ValidateAsync(ValidateAddressRequest request, CancellationToken ct = default);
}

public class AddressValidationService(HttpClient httpClient, IConfiguration configuration) : IAddressValidationService
{
    private const string BaseUrl = "https://addressvalidation.googleapis.com/v1:validateAddress";

    public async Task<GoogleValidateResponse> ValidateAsync(ValidateAddressRequest request, CancellationToken ct = default)
    {
        var apiKey = configuration["Google:AddressValidation:ApiKey"]
            ?? throw new InvalidOperationException("Google Address Validation API key is not configured.");

        var googleRequest = new GoogleValidateRequest(
            Address: new GooglePostalAddress(
                AddressLines: request.AddressLines,
                RegionCode: request.RegionCode,
                Locality: string.IsNullOrWhiteSpace(request.Locality) ? null : request.Locality,
                AdministrativeArea: string.IsNullOrWhiteSpace(request.AdministrativeArea) ? null : request.AdministrativeArea,
                PostalCode: string.IsNullOrWhiteSpace(request.PostalCode) ? null : request.PostalCode
            ),
            EnableUspsCass: string.Equals(request.RegionCode, "US", StringComparison.OrdinalIgnoreCase)
        );

        var response = await httpClient.PostAsJsonAsync($"{BaseUrl}?key={apiKey}", googleRequest, ct);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<GoogleValidateResponse>(ct)
            ?? throw new InvalidOperationException("Empty response from Google Address Validation API.");
    }
}
