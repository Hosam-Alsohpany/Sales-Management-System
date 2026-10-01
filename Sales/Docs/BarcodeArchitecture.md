# Barcode Architecture

## Goals
- Separate **identity** from **encoding** and from **parsing**.
- Support **progressive migration** without breaking existing POS data.
- Prevent repository/API explosion.

## Barcode Identity Model
A barcode identity is **(barcode_type, barcode_value)**.

- `NORMAL`: literal code scanned (e.g. EAN/UPC / internal labels). Stored as-is.
- `PLU`: price/weight lookup key for weighted items. Stored as digits only (no prefix).
- `INTERNAL`: reserved for internal/generated codes (future).

**Uniqueness** must be enforced on:
- `(barcode_type, barcode_value)`

## Encoding vs Identity
- **Identity** answers: “What key identifies the product unit?”
- **Encoding** answers: “How is extra data embedded in the scanned raw code?”
  - Example: EAN-13 weight code encodes `plu` + `price`.

## Resolver Flow (Raw -> Sale Line)
1. Input: `raw_code`.
2. Parse phase: profiles/parsers attempt to parse raw into a **payload**.
3. Lookup phase: lookup service maps payload into one or more identity lookups.
4. Migration fallback (only here): if new identity lookup fails, optionally try legacy identity.
5. Domain checks: optional domain validation (e.g. weighted only).
6. Qty calculation: qty computed from payload + product unit data.
7. Output: `BarcodeResolveResult` with `ProductUnitId` + `Qty` + debug metadata.

## Responsibility Boundaries
- Parsing (Profile/Parser):
  - Input: raw code
  - Output: structured payload (plu, priceRaw, canonicalCode...)
  - MUST NOT:
    - call DB
    - implement fallback/migration

- Lookup (Repository/Service):
  - Input: `BarcodeLookupRequest`
  - Output: `BarcodeLookupResult`
  - Owns:
    - mapping barcode identity to DB queries
    - enforcing `(barcode_type, barcode_value)` uniqueness expectation

- Orchestration (Resolver Engine/Orchestrator):
  - Owns:
    - selecting parser
    - choosing lookup strategy + fallback policy
    - logging warnings
    - building final `BarcodeResolveResult`

## Migration Strategy
- New data:
  - Weighted PLU stored as `(PLU, pluDigitsOnly)`
- Legacy data:
  - Some DBs may contain `(NORMAL, prefix+plu)` only
- Orchestrator policy:
  - Try `(PLU, plu)`
  - If not found and `AllowLegacyFallback=true`, try `(NORMAL, prefix+plu)`

