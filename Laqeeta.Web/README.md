# Laqeeta.Web

ASP.NET Core MVC frontend (Arabic, RTL) for **Laqeeta.API**.
It calls the API from the server side with `HttpClient`, so no CORS setup is needed.

## Run

1. Start the API first (profile `https`, it listens on https://localhost:7229).
2. Run this project (profile `https`): https://localhost:7300

```
dotnet run --project Laqeeta.Web --launch-profile https
```

If the API runs on another address, change `Laqeeta:ApiBaseUrl` in `appsettings.json`.
The list of stations is also in `appsettings.json` (the API has no endpoint for it).

## Pages

| Who | Page | URL |
|---|---|---|
| Passenger | New lost report | /LostReports/Create |
| Passenger | Track report + possible matches + claim | /Track |
| Passenger | Claim status | /Claims/Status |
| Staff | Dashboard | /Staff |
| Staff | Log a found item | /Staff/FoundItem |
| Staff | Verify / approve / reject / handover a claim | /Staff/ManageClaim |
| Admin | Stations report | /Admin |
| Staff + Admin | Login | /Account/Login |

Staff and Admin log in with the accounts the API seeds in Development (User Secrets: SeedStaff, SeedAdmin).
The JWT from the API is stored inside the login cookie (2 hours, same as the token).
