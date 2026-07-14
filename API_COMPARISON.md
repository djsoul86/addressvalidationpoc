# Address Validation POC — API Comparison

This document describes the three geolocation/address APIs implemented in this POC (`AddressValidationService`, `PlacesService`, `MapboxService`), summarizing the parameters each endpoint sends and what it returns, followed by a pricing comparison and a recommendation.

---

## 1. Google Address Validation API

- **Service:** `AddressValidationService.cs`
- **Endpoint:** `POST https://addressvalidation.googleapis.com/v1:validateAddress`
- **Auth:** API key as query param (`?key=...`)

### Request parameters

| Field | Type | Description |
|---|---|---|
| `address.addressLines` | `string[]` | Free-form address lines (street, unit, etc.) |
| `address.regionCode` | `string?` | ISO region code (e.g. `US`, `CA`) — improves parsing accuracy |
| `address.locality` | `string?` | City/town |
| `address.administrativeArea` | `string?` | State/province |
| `address.postalCode` | `string?` | Postal/ZIP code |
| `enableUspsCass` | `bool` | Requests USPS CASS certification data; in this implementation it's automatically set to `true` only when `regionCode == "US"` |

### Response

| Field | Description |
|---|---|
| `result.verdict` | Global validation outcome: `inputGranularity`, `validationGranularity`, `geocodeGranularity`, plus booleans `addressComplete`, `hasUnconfirmedComponents`, `hasInferredComponents`, `hasReplacedComponents` |
| `result.address` | Corrected `formattedAddress`, structured `postalAddress`, per-component breakdown (`addressComponents`) with confirmation level, spell-correction and replacement flags, plus `missingComponentTypes` / `unconfirmedComponentTypes` / `unresolvedTokens` |
| `result.geocode` | `location` (lat/lng), `plusCode`, `placeId`, `placeTypes` |
| `result.metadata` | Booleans: `business`, `poBox`, `residential` |
| `result.uspsData` | (US only) Standardized USPS address, delivery point code/check digit, DPV confirmation/footnotes, carrier route, county, CASS-processed flag, etc. |

This is the only one of the three APIs that returns a dedicated **verdict** on whether the address is real/deliverable, rather than just resolving text to a location.

---

## 2. Google Places API (New)

- **Service:** `PlacesService.cs`
- **Endpoints implemented:**
  - `POST https://places.googleapis.com/v1/places:searchText` (Text Search)
  - `POST https://places.googleapis.com/v1/places:searchNearby` (Nearby Search)
- **Auth:** headers `X-Goog-Api-Key` and `X-Goog-FieldMask` (field mask controls which fields are billed/returned)

> **Note on the pricing table below:** the endpoints implemented here (Text Search, Nearby Search) are billed under their own Places API SKUs, distinct from the **Geocoding** SKU referenced in the pricing comparison. The "Places API — Geocoding" line refers to calling Places API (New) purely to convert an address string into coordinates — administratively it is the **same underlying service and price as the standalone Geocoding API**, just billed under the Places API product umbrella. It does not correspond to a service call implemented in this POC, but it's the fair point of comparison since it's the cheapest way to get "text → coordinates" from Google.

### Request parameters

**Text Search** (`textQuery`-driven):

| Field | Type | Description |
|---|---|---|
| `textQuery` | `string` | Free-text query (e.g. address, place name) |
| `languageCode` | `string?` | Response language, default `en` |
| `regionCode` | `string?` | Biases results to a region |
| `maxResultCount` | `int` | Max results, default 10 |
| `locationBias.circle` | `{center, radius}` | Optional lat/lng + radius (meters) to bias results toward an area |

**Nearby Search** (`locationRestriction`-driven):

| Field | Type | Description |
|---|---|---|
| `locationRestriction.circle` | `{center, radius}` | Required lat/lng + radius (meters); results are restricted to this area |
| `includedTypes` / `excludedTypes` | `string[]?` | Filter by Google place types (e.g. `restaurant`, `cafe`) |
| `includedPrimaryTypes` | `string[]?` | Filter by primary type only |
| `maxResultCount` | `int` | Max results, default 10 |
| `languageCode` / `regionCode` | `string?` | Same as above |

### Response

Both endpoints return a `places[]` array (field-masked in this implementation to):

| Field | Description |
|---|---|
| `id`, `displayName` | Place ID and localized name |
| `formattedAddress`, `shortFormattedAddress` | Human-readable address |
| `location` | Lat/lng |
| `types`, `primaryType`, `primaryTypeDisplayName` | Place category |
| `rating`, `userRatingCount`, `priceLevel`, `businessStatus` | Business metadata |
| `regularOpeningHours` | Hours (periods + weekday descriptions) |
| `nationalPhoneNumber`, `internationalPhoneNumber`, `websiteUri`, `googleMapsUri` | Contact info |
| `editorialSummary`, `plusCode` | Extra descriptive data |
| `nextPageToken` | Pagination cursor |

Places API never confirms an address is *valid* or *deliverable* — it only resolves a query to place(s) with coordinates.

---

## 3. Mapbox Search Box API

- **Service:** `MapboxService.cs`
- **Base URL:** `https://api.mapbox.com/search/searchbox/v1`
- **Endpoints implemented:**
  - `GET /suggest` — autocomplete suggestions (step 1 of 2)
  - `GET /retrieve/{mapbox_id}` — full details for a suggestion (step 2 of 2)
  - `GET /forward` — single-step forward geocoding (suggest + retrieve combined)
- **Auth:** `access_token` query param; every call also sends a `session_token` (UUID) to group suggest+retrieve calls for billing purposes

> **Note on the pricing table below:** this POC uses Mapbox's **Search Box API** (the current/recommended product for address search and autocomplete), not the older standalone **Geocoding API**. Mapbox bills them under separate SKUs with different pricing. The comparison below uses the **Geocoding (Temporary)** pricing tier as provided, which is the closest published equivalent for "resolve an address string to coordinates + confidence" — validate actual Search Box pricing separately before committing to a cost model.

### Request parameters

| Field | Type | Description |
|---|---|---|
| `q` (`Query`) | `string` | Free-text search string |
| `session_token` (`SessionToken`) | `string` (UUID) | Groups a suggest→retrieve pair for billing as one session |
| `country` | `string?` | ISO country code filter |
| `language` | `string` | Response language, default `en` |
| `limit` | `int` | Max results, default 5 |
| `proximity` (`ProximityLatitude`/`Longitude`) | `double?` | Biases results near a point |
| `types` | `string[]?` | Restrict to feature types (e.g. `address`) — `/suggest` only |
| `mapbox_id` (`MapboxId`) | `string` | ID returned by `/suggest`, used by `/retrieve` |

### Response

`/suggest` returns a lightweight `suggestions[]` array (name, `mapbox_id`, `feature_type`, `place_formatted`, `full_address`, `context`) — no coordinates yet, by design (keeps autocomplete cheap).

`/retrieve` and `/forward` return a GeoJSON `FeatureCollection`:

| Field | Description |
|---|---|
| `features[].geometry.coordinates` | `[longitude, latitude]` |
| `features[].properties.name`, `full_address`, `place_formatted` | Address text |
| `features[].properties.coordinates` | `{longitude, latitude, accuracy, routable_points}` — `accuracy` reflects match confidence (e.g. `rooftop`, `parcel`, `street`) |
| `features[].properties.context` | Structured breakdown: `address` (number + street), `street`, `neighborhood`, `postcode`, `place`, `district`, `region`, `country` |
| `features[].properties.bbox` | Bounding box |

---

## Pricing Comparison

With the Places API added to the comparison (keep in mind: the "Geocoding" SKU inside Places API (New) is the same underlying service as the standalone Geocoding API, just billed under a different administrative name):

| | Google Address Validation (Pro) | Google Places API — Geocoding | Mapbox Geocoding (Temporary) |
|---|---|---|---|
| Free tier/month | 5,000 | 10,000 | 100,000 |
| Cost (up to 100K) | $17.00 / 1,000 | $5.00 / 1,000 | $0.75 / 1,000 |
| Verifies the address exists? | Yes — dedicated verdict, corrects and standardizes | No — only resolves text → coordinates, no deliverability confirmation | Yes — match_code + confidence per component (Smart Address Match, v6) |
| Official postal certification (e.g. USPS CASS) | Enterprise tier only | No | No |
| What it returns | Corrected address + overall confidence level | Lat/lng + formatted address, no validity judgment | Address + confidence score ("exact" → "low") + which parts matched |

### Conclusion for your case

(validating that the user entered a real address, without needing postal certification):

- **Places API - Geocoding is ruled out** for this: it doesn't validate anything, it only locates. It's the cheapest option but doesn't solve your problem.
- The real decision is between **Address Validation Pro** and **Mapbox Geocoding**. Mapbox clearly wins on cost (22x cheaper, 20x more free tier) and already ships with a confidence score good enough for most form-validation use cases. Google Address Validation only pays off if you need the level of "official" assurance its verdict provides (e.g. for logistics/shipping, where a failed delivery is expensive) or USPS CASS certification in the US.

**Recommendation for the POC:** test Mapbox Geocoding first against a real sample of your addresses — it's free up to 100K/month and will quickly tell you whether the confidence score is reliable enough for your use case before spending on the Google option.
