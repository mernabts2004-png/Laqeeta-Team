using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Laqeeta.Web.Models;
using Microsoft.AspNetCore.Authentication;

namespace Laqeeta.Web.Services;

// One place that knows every endpoint of Laqeeta.API.
public class LaqeetaApiClient(HttpClient http, IHttpContextAccessor accessor)
{
    private static readonly JsonSerializerOptions JsonOptions =
        new(JsonSerializerDefaults.Web)
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

    // ---------- Auth ----------

    public Task<ApiResult<LoginResponse>> LoginAsync(string email, string password) =>
        SendAsync<LoginResponse>(
            HttpMethod.Post,
            "api/Auth/login",
            new LoginRequest { Email = email, Password = password });

    // ---------- Passenger (public) ----------

    public Task<ApiResult<ReportCreatedResponse>> CreateLostReportAsync(
        CreateLostReportRequest request) =>
        SendAsync<ReportCreatedResponse>(HttpMethod.Post, "api/LostReports", request);

    public Task<ApiResult<List<PossibleMatchResponse>>> GetPossibleMatchesAsync(
        int reportId, string phoneNumber) =>
        SendAsync<List<PossibleMatchResponse>>(
            HttpMethod.Post,
            "api/PossibleMatches",
            new ReportLookupRequest { ReportId = reportId, PhoneNumber = phoneNumber });

    public Task<ApiResult<ReportStatusResponse>> GetReportStatusAsync(
        int reportId, string phoneNumber) =>
        SendAsync<ReportStatusResponse>(
            HttpMethod.Post,
            "api/ReportStatus",
            new ReportLookupRequest { ReportId = reportId, PhoneNumber = phoneNumber });

    public Task<ApiResult<ClaimResponse>> CreateClaimAsync(
        int reportId, int itemId, string phoneNumber) =>
        SendAsync<ClaimResponse>(
            HttpMethod.Post,
            "api/Claims",
            new CreateClaimRequest
            {
                ReportId = reportId,
                ItemId = itemId,
                PhoneNumber = phoneNumber
            });

    public Task<ApiResult<ClaimResponse>> GetClaimStatusAsync(
        int claimId, string phoneNumber) =>
        SendAsync<ClaimResponse>(
            HttpMethod.Post,
            "api/Claims/status",
            new ClaimStatusRequest { ClaimId = claimId, PhoneNumber = phoneNumber });

    // ---------- Staff ----------

    public Task<ApiResult<FoundItemCreatedResponse>> CreateFoundItemAsync(
        CreateFoundItemRequest request) =>
        SendAsync<FoundItemCreatedResponse>(
            HttpMethod.Post, "api/FoundItems", request, authorize: true);

    public Task<ApiResult<SecretQuestionResponse>> GetSecretQuestionAsync(int claimId) =>
        SendAsync<SecretQuestionResponse>(
            HttpMethod.Get,
            $"api/Claims/{claimId}/secret-question",
            authorize: true);

    public Task<ApiResult<string>> VerifyClaimAsync(
        int claimId, bool idVerified, string? secretAnswer) =>
        SendAsync<string>(
            HttpMethod.Post,
            $"api/Claims/{claimId}/verify",
            new VerifyClaimRequest { IdVerified = idVerified, SecretAnswer = secretAnswer },
            authorize: true);

    public Task<ApiResult<string>> ApproveClaimAsync(int claimId) =>
        SendAsync<string>(
            HttpMethod.Post, $"api/Claims/{claimId}/approve", authorize: true);

    public Task<ApiResult<string>> RejectClaimAsync(int claimId) =>
        SendAsync<string>(
            HttpMethod.Post, $"api/Claims/{claimId}/reject", authorize: true);

    public Task<ApiResult<ClaimResponse>> HandoverClaimAsync(int claimId) =>
        SendAsync<ClaimResponse>(
            HttpMethod.Post, $"api/Claims/{claimId}/handover", authorize: true);

    // ---------- Admin ----------

    public Task<ApiResult<List<StationReportResponse>>> GetStationReportAsync() =>
        SendAsync<List<StationReportResponse>>(
            HttpMethod.Get, "api/AdminReports/stations", authorize: true);

    // ---------- Core ----------

    private async Task<ApiResult<T>> SendAsync<T>(
        HttpMethod method,
        string url,
        object? body = null,
        bool authorize = false)
    {
        try
        {
            using var request = new HttpRequestMessage(method, url);

            if (body is not null)
            {
                var json = JsonSerializer.Serialize(body, body.GetType(), JsonOptions);
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            }

            if (authorize)
            {
                var context = accessor.HttpContext;

                var token = context is null
                    ? null
                    : await context.GetTokenAsync("access_token");

                if (string.IsNullOrWhiteSpace(token))
                {
                    return ApiResult<T>.Fail(
                        HttpStatusCode.Unauthorized,
                        "لازم تسجّل دخول الأول.");
                }

                request.Headers.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            using var response = await http.SendAsync(request);

            var text = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return ApiResult<T>.Fail(
                    response.StatusCode,
                    ExtractError(response.StatusCode, text));
            }

            // Some endpoints return a plain-text confirmation message.
            if (typeof(T) == typeof(string))
            {
                return ApiResult<T>.Ok((T)(object)ReadPlainText(text), response.StatusCode);
            }

            var data = JsonSerializer.Deserialize<T>(text, JsonOptions);

            if (data is null)
            {
                return ApiResult<T>.Fail(
                    response.StatusCode,
                    "الـ API رجّع رد فاضي أو غير مفهوم.");
            }

            return ApiResult<T>.Ok(data, response.StatusCode);
        }
        catch (HttpRequestException)
        {
            return ApiResult<T>.Fail(
                0,
                "مش قادر أوصل للـ API. اتأكد إن مشروع Laqeeta.API شغال وإن العنوان في appsettings.json صح.");
        }
        catch (TaskCanceledException)
        {
            return ApiResult<T>.Fail(
                0,
                "الـ API اتأخر في الرد. جرّب تاني.");
        }
        catch (JsonException)
        {
            return ApiResult<T>.Fail(
                0,
                "الرد اللي جه من الـ API مش بالشكل المتوقع.");
        }
    }

    private static string ReadPlainText(string text)
    {
        var trimmed = text.Trim();

        if (trimmed.Length >= 2 && trimmed[0] == '"' && trimmed[^1] == '"')
        {
            try
            {
                return JsonSerializer.Deserialize<string>(trimmed) ?? trimmed;
            }
            catch (JsonException)
            {
                // Fall through and return the text as it is.
            }
        }

        return trimmed;
    }

    private static string ExtractError(HttpStatusCode status, string body)
    {
        var text = (body ?? string.Empty).Trim();

        // Validation errors come back as ProblemDetails JSON.
        if (text.StartsWith('{'))
        {
            try
            {
                using var document = JsonDocument.Parse(text);
                var root = document.RootElement;

                if (root.TryGetProperty("errors", out var errors) &&
                    errors.ValueKind == JsonValueKind.Object)
                {
                    var messages = new List<string>();

                    foreach (var property in errors.EnumerateObject())
                    {
                        if (property.Value.ValueKind != JsonValueKind.Array)
                        {
                            continue;
                        }

                        foreach (var item in property.Value.EnumerateArray())
                        {
                            var message = item.GetString();

                            if (!string.IsNullOrWhiteSpace(message))
                            {
                                messages.Add(message);
                            }
                        }
                    }

                    if (messages.Count > 0)
                    {
                        return string.Join(" — ", messages);
                    }
                }

                if (root.TryGetProperty("title", out var title) &&
                    title.ValueKind == JsonValueKind.String &&
                    !string.IsNullOrWhiteSpace(title.GetString()))
                {
                    return title.GetString()!;
                }
            }
            catch (JsonException)
            {
                // Not valid JSON, treat it as plain text below.
            }
        }

        if (text.Length == 0)
        {
            return status switch
            {
                HttpStatusCode.Unauthorized => "انتهت الجلسة أو مفيش صلاحية. سجّل دخول تاني.",
                HttpStatusCode.Forbidden => "مش مسموحلك تعمل العملية دي.",
                HttpStatusCode.NotFound => "العنصر المطلوب مش موجود.",
                _ => $"حصل خطأ غير متوقع (HTTP {(int)status})."
            };
        }

        return ErrorTranslator.Translate(text);
    }
}
