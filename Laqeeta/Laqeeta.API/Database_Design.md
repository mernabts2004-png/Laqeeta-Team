# Laqeeta Database Design

Database: SQL Server, managed with EF Core migrations. Source of truth: Data/ApplicationDbContext.cs and the files in Models/.
Identity tables (users, roles) are created by ASP.NET Core Identity and hold the Staff and Admin accounts.

## Tables

### Stations
| Column | Type | Notes |
|---|---|---|
| StationId | int | Primary key |
| Name | nvarchar(100) | Unique |

### Items (found items)
| Column | Type | Notes |
|---|---|---|
| ItemId | int | Primary key |
| Category | nvarchar(50) | |
| Color | nvarchar(50) | |
| Line | nvarchar(50) | |
| Direction | nvarchar(100) | |
| FoundAt | datetime2 | Stored in UTC |
| Description | nvarchar(500) | |
| FoundAtStationId | int | Foreign key to Stations (Restrict delete) |
| StorageLocation | nvarchar(30) | Station or CentralStoreroom |
| MovedToCentralAt | datetime2 null | Set when moved to the central storeroom |
| Status | nvarchar(30) | Found, Reserved, Returned |

Index: (Category, Line, FoundAt) for matching.
Check constraints: valid StorageLocation, valid Status, and MovedToCentralAt must be set only when the item is in the CentralStoreroom.

### Reports (lost-item reports)
| Column | Type | Notes |
|---|---|---|
| ReportId | int | Primary key |
| PhoneNumber | nvarchar(20) | Cleaned digits only |
| Category, Color, Line, Direction | nvarchar | Same sizes as Items |
| LostAt | datetime2 | Stored in UTC |
| Description | nvarchar(500) | |
| Status | nvarchar(30) | Searching, PossibleMatch, ReadyToCollect, Closed |
| CreatedAt | datetime2 | |
| SecretQuestion | nvarchar(200) null | Required for high-value items |
| SecretAnswerHash | nvarchar(500) null | Hashed with ASP.NET Identity PasswordHasher, never returned by the API |

Index: (Category, Line, LostAt). Check constraint: valid Status.

### Claims
| Column | Type | Notes |
|---|---|---|
| ClaimId | int | Primary key |
| ReportId | int | Foreign key to Reports (Restrict delete) |
| ItemId | int | Foreign key to Items (Restrict delete) |
| Status | nvarchar(30) | Pending, Rejected, Approved, Completed |
| IdVerified | bit | Staff checked the ID card |
| SecretQuestionVerified | bit | Secret answer verified (or not required) |
| VerifiedAt | datetime2 null | |
| HandedOverAt | datetime2 null | Set only when Status is Completed |

Unique index: (ReportId, ItemId), so the same report cannot claim the same item twice.
Unique filtered index: ItemId where Status = Completed, so an item cannot be handed over twice.
Check constraints: valid Status, and HandedOverAt must be set only for Completed claims.

## Relationships

- Station 1 -> N Items (Items.FoundAtStationId)
- Report 1 -> N Claims (Claims.ReportId)
- Item 1 -> N Claims (Claims.ItemId)
- Reports and Items are connected only through Claims (many-to-many with extra columns).

erDiagram
    Stations ||--o{ Items : "found at"
    Reports ||--o{ Claims : "has"
    Items ||--o{ Claims : "is claimed in"
## Seed data

Sample stations, found items and lost reports are inserted by migrations (HasData). Staff and Admin users are created at startup in the Development environment from User Secrets (SeedStaff, SeedAdmin).

## Business rules enforced in code (not in the database)

- Items older than 30 days with status Found are moved to the central storeroom by a background service that runs every 24 hours.
- Only one claim can be approved for each item.
- High-value reports must have a secret question and answer.