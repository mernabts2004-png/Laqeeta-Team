# Laqeeta API Endpoints

Base URL (local): https://localhost:7229
All JSON field names use camelCase. Dates are Egypt local time without Z or offset (example: 2026-11-05T14:30:00).
Success responses return 200 OK. Error messages are returned as plain text (validation errors return the standard ProblemDetails JSON).

Authentication: send Authorization: Bearer <token> for Staff and Admin endpoints. The token is returned by /api/Auth/login and expires after 2 hours.

## Auth

| Method | URL | Access | What it does |
|---|---|---|---|
| POST | /api/Auth/login | Public | Logs in a Staff or Admin user. Body: email, password. Returns token, tokenType, expiresAt, fullName, roles. Wrong data returns 401. |

## Passenger endpoints (public, no login)

The passenger is identified by reportId + phoneNumber. Phone numbers are cleaned before use (spaces, dashes and brackets are removed, Arabic digits are converted).

| Method | URL | What it does |
|---|---|---|
| POST | /api/LostReports | Creates a lost-item report. Body: phoneNumber, category, color, line, direction, lostAt, description, optional secretQuestion + secretAnswer. Returns reportId, status, createdAt. |
| POST | /api/PossibleMatches | Returns found items that may match the report. Body: reportId, phoneNumber. Returns a list of itemId, category, color, line, direction, foundAt, description, stationName. Also updates the report status. Closed reports return 400. |
| POST | /api/ReportStatus | Returns the report status. Body: reportId, phoneNumber. Returns reportId, status, createdAt. Refreshes Searching / PossibleMatch before answering. |
| POST | /api/Claims | Passenger asks to collect an item. Body: reportId, itemId, phoneNumber. Returns claimId, reportId, itemId, status, handedOverAt. |
| POST | /api/Claims/status | Returns the claim status. Body: claimId, phoneNumber. |

## Staff endpoints (role: Staff)

| Method | URL | What it does |
|---|---|---|
| POST | /api/FoundItems | Logs a found item. Body: category, color, line, direction, foundAt, description, foundAtStationId. Returns itemId, status, storageLocation, foundAt. |
| GET | /api/Claims/{claimId}/secret-question | Returns the secret question of the claimed report (never the answer). Only for Pending claims. |
| POST | /api/Claims/{claimId}/verify | Verifies the owner. Body: idVerified (true after the staff checks the ID card), secretAnswer (required for high-value items). |
| POST | /api/Claims/{claimId}/approve | Approves a verified claim. The item becomes Reserved and the report becomes ReadyToCollect. |
| POST | /api/Claims/{claimId}/reject | Rejects a Pending claim. |
| POST | /api/Claims/{claimId}/handover | Hands the item to the owner. The claim becomes Completed, the item Returned, the report Closed. |

## Admin endpoints (role: Admin)

| Method | URL | What it does |
|---|---|---|
| GET | /api/AdminReports/stations | Per station: stationId, stationName, foundItemsCount, returnedItemsCount, returnRate (percent). |

## Status values

- Report: Searching -> PossibleMatch -> ReadyToCollect -> Closed
- Claim: Pending -> Approved -> Completed (or Rejected from Pending)
- Item: Found -> Reserved -> Returned

## Matching rule (short)

A found item is a possible match when it has the same category, line and direction as the report, was found within 3 days before or after the lost date, and its status is Found. See MatchingRule.md.

## High-value items

If the category contains a high-value word (phone, wallet, ID...), the report must include a secret question and answer, and staff must check the answer before approving. The answer is not case-sensitive for reports created after the latest update.

## Known limitations (to be fixed or agreed with Frontend)- There is no endpoint that lists stations, so the frontend uses a fixed list.
- There is no endpoint that lists pending claims; staff enter the claim number.
- category, line and direction are free text.
- No rate limiting or login lockout yet.